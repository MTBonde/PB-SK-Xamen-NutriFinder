using ClosedXML.Excel;
using NutriFinder.Server.Interfaces;
using Nutrifinder.Shared;

namespace NutriFinder.Server.External
{
    /// <summary>
    /// Abstraction over Excel nutrition lookup to isolate ClosedXML dependency.
    /// </summary>
    public interface IExcelNutritionReader
    {
        NutritionDTO? Lookup(string foodItemName);
    }

    /// <summary>
    /// Adapter implementation of INutritionExternalApi using IExcelNutritionReader.
    /// </summary>
    public class DTUNutritionExternalAPI : INutritionExternalApi
    {
        private readonly IExcelNutritionReader _reader;

        public DTUNutritionExternalAPI(IExcelNutritionReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public Task<NutritionDTO?> FetchNutritionDataAsync(string foodItemName)
        {
            if (string.IsNullOrWhiteSpace(foodItemName))
            {
                Console.Error.WriteLine("Input food name is null or whitespace.");
                return Task.FromResult<NutritionDTO?>(null);
            }

            var dto = _reader.Lookup(foodItemName.Trim());
            return Task.FromResult(dto);
        }
    }

    /// <summary>
    /// Real implementation that uses ClosedXML to read from an Excel file.
    /// </summary>
    public class ExcelNutritionReader : IExcelNutritionReader
    {
        private readonly string _filePath;

        private const string FoodSheetName = "Food";
        private const string DataSheetName = "Data_Normalised";
        private const string FoodIdColumn = "FoodID";
        private const string FoodNameColumn = "FoodName";
        private const string ParameterNameColumn = "ParameterName";
        private const string ResultValueColumn = "ResVal";

        private static readonly Dictionary<string, string> ExternalParameterToDtoFieldMapping = new()
        {
            { "Carbohydrate, available", "Carb" },
            { "Dietary fibre", "Fiber" },
            { "Protein", "Protein" },
            { "Fat", "Fat" },
            { "Energy (kcal)", "Kcal" }
        };

        public ExcelNutritionReader(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public NutritionDTO? Lookup(string foodItemName)
        {
            Console.WriteLine($"[ExcelReader] Attempting lookup for: '{foodItemName}'");

            try
            {
                using var workbook = new XLWorkbook(_filePath);

                var foodSheet = workbook.Worksheet(FoodSheetName);
                if (foodSheet == null)
                {
                    Console.Error.WriteLine($"Sheet '{FoodSheetName}' not found in file '{_filePath}'.");
                    return null;
                }

                var foodTable = foodSheet.RangeUsed().AsTable();

                var trimmedFoodName = foodItemName.Trim();

                // foreach (var row in foodTable.DataRange.Rows())
                // {
                //     var name = row.Field(FoodNameColumn)?.GetString()?.Trim();
                //     Console.WriteLine($"Found food name: '{name}'");
                // }

                // Try partial match 
                var foodRow = foodTable.DataRange.Rows()
                    .FirstOrDefault(r => r.Field(FoodNameColumn)
                        ?.GetString()
                        ?.Trim()
                        .IndexOf(trimmedFoodName,
                            StringComparison.OrdinalIgnoreCase) >= 0);


                if (foodRow == null)
                {
                    Console.Error.WriteLine($"Food '{trimmedFoodName}' not found in '{FoodSheetName}'.");
                    return null;
                }

                int foodId;
                try
                {
                    foodId = Convert.ToInt32(foodRow.Field(FoodIdColumn).GetDouble());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error parsing FoodID for '{trimmedFoodName}': {ex}");
                    return null;
                }

                var dataSheet = workbook.Worksheet(DataSheetName);
                if (dataSheet == null)
                {
                    Console.Error.WriteLine($"Sheet '{DataSheetName}' not found in file '{_filePath}'.");
                    return null;
                }

                var dataTable = dataSheet.RangeUsed().AsTable();

                // Lookup all required nutrients.
                var nutrientResults = new Dictionary<string, float>();

                foreach (var (externalParam, dtoField) in ExternalParameterToDtoFieldMapping)
                {
                    var dataRow = dataTable.DataRange.Rows()
                        .FirstOrDefault(r =>
                            Convert.ToInt32(r.Field(FoodIdColumn).GetDouble()) == foodId &&
                            string.Equals(r.Field(ParameterNameColumn)?.GetString()?.Trim(), externalParam,
                                StringComparison.OrdinalIgnoreCase)
                        );
                    if (dataRow != null &&
                        float.TryParse(dataRow.Field(ResultValueColumn)?.GetString(), out float value))
                    {
                        nutrientResults[dtoField] = value;
                    }
                    else if (dataRow != null && dataRow.Field(ResultValueColumn).TryGetValue(out double dval))
                    {
                        nutrientResults[dtoField] = (float)dval;
                    }
                    else
                    {
                        nutrientResults[dtoField] = 0f; // Not found, or could not parse
                    }
                }

                // Build DTO and return result.
                return new NutritionDTO
                {
                    FoodItemName = trimmedFoodName,
                    Carb = nutrientResults["Carb"],
                    Fiber = nutrientResults["Fiber"],
                    Protein = nutrientResults["Protein"],
                    Fat = nutrientResults["Fat"],
                    Kcal = nutrientResults["Kcal"]
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during lookup for '{foodItemName}': {ex}");
                return null;
            }
        }
    }
}
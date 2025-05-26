using ClosedXML.Excel;
using NutriFinder.Server.Interfaces;
using Nutrifinder.Shared;

namespace NutriFinder.Server.External
{
    public class DTUNutritionExternalAPI : INutritionExternalApi
    {
        // Constants for all sheet and column names.
        // data normalized is better format "tidy"
        // we can find food id in the food sheet
        // all curently looked for nutrition is in g/100g in resval
        // resval for energy is in kg cal pr 100g
        private const string FoodSheetName = "Food";
        private const string DataSheetName = "Data_Normalised";
        private const string FoodIdColumn = "FoodID";
        private const string FoodNameColumn = "FoodName";
        private const string ParameterNameColumn = "ParameterName";
        private const string ResultValueColumn = "ResVal";

        // Mapping from Excel parameter names to DTO properties.
        private static readonly Dictionary<string, string> ExternalParameterToDtoFieldMapping = new()
        {
            { "Carbohydrate, available", "Carb" },
            { "Dietary fibre", "Fiber" },
            { "Protein", "Protein" },
            { "Fat", "Fat" },
            { "Energy (kcal)", "Kcal" }
        };

        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the FridaNutritionExternalApi class.
        /// </summary>
        /// <param name="filePath">Path to the Excel file containing nutrition data.</param>
        public DTUNutritionExternalAPI(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        /// <summary>
        /// Looks up nutrition data for a given food name by reading the Excel file on each call.
        /// </summary>
        /// <param name="foodItemName">The English name of the food to look up.</param>
        /// <returns>NutritionDTO if found; otherwise null.</returns>
        public async Task<NutritionDTO?> FetchNutritionDataAsync(string foodItemName)
        {
            if (string.IsNullOrWhiteSpace(foodItemName))
            {
                Console.Error.WriteLine("Input food name is null or whitespace.");
                return null;
            }

            // Task.Run used because ClosedXML is synchronous and may block request threads.
            return await Task.Run(() =>
            {
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

                    // Find matching Food row by English name.
                    var foodRow = foodTable.DataRange.Rows()
                        .FirstOrDefault(r =>
                            string.Equals(
                                r.Field(FoodNameColumn)?.GetString()?.Trim(),
                                trimmedFoodName,
                                StringComparison.OrdinalIgnoreCase)
                        );

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
            });
        }
    }
}
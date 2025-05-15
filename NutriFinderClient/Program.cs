using System.Diagnostics.CodeAnalysis;

namespace NutriFinderClient;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new NutritionClient();

        while (true)
        {
            // ask user for input
            Console.WriteLine(" Enter food item in english");
            var userInput = Console.ReadLine();
            var validationResult = client.ValidateInput(userInput);
            
            // EO; if not valid
            if (validationResult != "ok")
            {
                Console.WriteLine($"Input error: {validationResult}");
                continue;
            }
            
            /*// fake HTTP-Respons
            var statusCode; 
            NutritionDTO? nutritionData = new NutritionDTO
            {
                FoodItemName = userInput,
                Carb = 100,
                Fiber = 10,
                Protein = 5,
                Fat = 1,
                Kcal = 250,
            };
            
            if (statusCode == 200 && nutritionData != null)
            {
                Console.WriteLine(client.FormatNutritionOutput(nutritionData));
                break;
            }
            else
            {
                Console.WriteLine(client.FormatErrorMessageFromStatusCode(statusCode));
            }*/
            
            NutritionDTO? dto = await client.FetchNutritionDataAsync(userInput);
    
            if (dto == null)
            {
                Console.WriteLine("Failed to fetch data.");
                continue;
            }

            Console.WriteLine(client.FormatNutritionOutput(dto));
            break;
        }
    }
}
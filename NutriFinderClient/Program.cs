using System.Diagnostics.CodeAnalysis;
using Nutrifinder.Shared;

namespace NutriFinderClient;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        bool shouldLoop = true;
        var client = new NutritionClient();
        
        Console.WriteLine("Hello and welcome to NutriFinder!\n" +
                          "Simply type any food item you wish to retrieve nutritional data about the desired food!\n");

        while (shouldLoop)
        {
            // ask user for input
            Console.WriteLine("Enter food item in english:");
            var userInput = Console.ReadLine();
            
            Console.WriteLine();
            
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
            
            // NutritionDTO? dto = await client.FetchNutritionDataAsync(userInput);
            var dto = await FetchWithAnimationAsync(client, userInput);
    
            if (dto == null)
            {
                Console.WriteLine("Failed to fetch data! Please try again!\n" +
                                  "(Did you spell the food item correctly?)\n");
                continue;
            }
            
            Console.WriteLine("-------------------------------------");
            
            Console.WriteLine("Results: \n");
            Console.WriteLine(client.FormatNutritionOutput(dto) + "\n");
            
            Console.WriteLine("-------------------------------------");
            
            //Wants to loop? 
            Console.WriteLine("Would you like to fetch another food item? (y/n)");
            bool inputValid = false;
            
            while (inputValid == false)
            {
                var loopInput = Console.ReadLine();
                
                if (loopInput == "y")
                {
                    //Loop
                    inputValid = true;
                    shouldLoop = true;
                    Console.WriteLine();
                }
                else if (loopInput == "n")
                {
                    inputValid = true;
                    shouldLoop = false;
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Invalid input! Please only type Y or N!");
                    inputValid = false;
                }
            }
            
            if(shouldLoop == false)
            {
                break;
            }
        }
    }

    static async Task<NutritionDTO?> FetchWithAnimationAsync(NutritionClient client, string userInput, int timeoutSeconds = 30)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
    
        // Start animation i en separat task
        var animationTask = Task.Run(async () =>
        {
            try
            {
                int animFrame = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.Write("\rFetching data" + new string('.', animFrame));
                    Console.Write(new string(' ', 3 - animFrame));
                    animFrame = (animFrame + 1) % 4;
                    await Task.Delay(300, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignorer TaskCanceledException her - det er forventet når vi afbryder
            }
        });

        try
        {
            // Hent data
            var dto = await client.FetchNutritionDataAsync(userInput);
        
            // Stop animationen og ryd op
            cts.Cancel();
            await animationTask;
            Console.WriteLine();
        
            return dto;
        }
        catch (Exception ex)
        {
            // Stop animationen og ryd op
            cts.Cancel();
            await animationTask;
            Console.WriteLine();
        
            // Log fejlen hvis nødvendigt
            Console.WriteLine($"\nFejl ved hentning af data: {ex.Message}");
            return null;
        }
    }
}
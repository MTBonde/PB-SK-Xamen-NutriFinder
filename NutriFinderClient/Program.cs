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
            Console.WriteLine("Enter food item in english:");
            var userInput = Console.ReadLine();

            if (userInput == null)
            {
                Console.WriteLine("No input received – please try again.\n");
                continue;
            }

            Console.WriteLine();

            var validationResult = client.ValidateInput(userInput);

            if (validationResult != "ok")
            {
                Console.WriteLine($"Input error: {validationResult}");
                continue;
            }

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

            Console.WriteLine("Would you like to fetch another food item? (y/n)");
            bool inputValid = false;

            while (!inputValid)
            {
                var loopInput = Console.ReadLine();

                if (loopInput == "y")
                {
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

            if (!shouldLoop)
            {
                break;
            }
        }
    }

    private static async Task<NutritionDTO?> FetchWithAnimationAsync(NutritionClient client, string userInput, int timeoutSeconds = 30)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var token = cts.Token;

        var animationTask = Task.Run(async () =>
        {
            try
            {
                int animFrame = 0;
                while (!token.IsCancellationRequested)
                {
                    Console.Write("\rFetching data" + new string('.', animFrame));
                    Console.Write(new string(' ', 3 - animFrame));
                    animFrame = (animFrame + 1) % 4;
                    await Task.Delay(300, token);
                }
            }
            catch (OperationCanceledException)
            {
                // expected
            }
        }, token);

        try
        {
            var dto = await client.FetchWithFallbackBaseUrlsAsync(userInput);

            cts.Cancel();
            await animationTask;
            Console.WriteLine();

            Console.WriteLine(dto != null
                ? "[INFO] Data fetch complete."
                : "[WARN] No data could be retrieved from any source.");

            return dto;
        }
        catch (Exception ex)
        {
            cts.Cancel();
            Console.WriteLine();
            Console.WriteLine($"\n[ERROR] Exception during fetch: {ex.Message}");
            return null;
        }
    }
}

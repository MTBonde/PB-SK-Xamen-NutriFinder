using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Nutrifinder.Shared;

namespace NutriFinderClient;

public class NutritionClient(HttpClient httpClient)
{
    public NutritionClient() : this(new HttpClient())
    {
    }

    public string ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Input can not be empty";

        if (input.Any(char.IsDigit))
            return "Input can not contain numbers";

        return !Regex.IsMatch(input, "^[a-åA-Å]+$") ? "Only English letters is accepted" : "ok";
    }

    public string FormatNutritionOutput(NutritionDTO? dto)
    {
        if (dto != null)
        {
            return $"""
                    Food: {dto.FoodItemName}
                    Carbohydrates: {dto.Carb} g
                    Fiber: {dto.Fiber} g
                    Net Carbohydrates: {dto.Carb - dto.Fiber} g
                    Protein: {dto.Protein} g
                    Fat: {dto.Fat} g
                    Calories: {dto.Kcal} kcal
                    """;
        }

        return "[ERROR] Cannot format null nutrition data.";
    }

    public string? FormatErrorMessageFromStatusCode(int expectedStatusCode)
    {
        return expectedStatusCode switch
        {
            200 => "Success: OK!",
            400 => "Error: Bad request!",
            404 => "Error: Food item not found",
            503 => "Error: External API is not available and no cached data was found.",
            _ => null
        };
    }

    public async Task<NutritionDTO?> FetchNutritionDataAsync(string query)
    {
        var request = $"/api/nutrition?foodItemName={query}";
        var response = await httpClient.GetAsync(request);

        Console.WriteLine(response.StatusCode + " with query: " + request);

        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<NutritionDTO>();
    }

    public async Task<NutritionDTO?> FetchWithFallbackBaseUrlsAsync(string query)
    {
        if (AppDomain.CurrentDomain.FriendlyName.Contains("testhost"))
        {
            Console.WriteLine("[TEST] Using injected HttpClient (testhost context). Trying directly.");
            return await FetchNutritionDataAsync(query);
        }

        var fallbackBaseUrls = new[]
        {
            "https://api.mtbonde.dev",
            "http://localhost:5000"
        };

        foreach (var baseUrl in fallbackBaseUrls)
        {
            try
            {
                Console.WriteLine($"[INFO] Attempting to contact: {baseUrl}");

                using var localClient = new HttpClient();
                localClient.BaseAddress = new Uri(baseUrl);
                var request = $"/api/nutrition?foodItemName={query}";
                var response = await localClient.GetAsync(request);

                Console.WriteLine($"[INFO] Received {response.StatusCode} from {baseUrl}");

                if (!response.IsSuccessStatusCode) continue;

                Console.WriteLine($"[SUCCESS] Successfully fetched data from: {baseUrl}");
                return await response.Content.ReadFromJsonAsync<NutritionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed contacting {baseUrl}: {ex.Message}");
            }
        }

        Console.WriteLine("[FAILURE] All sources failed. No data returned.");
        return null;
    }
}

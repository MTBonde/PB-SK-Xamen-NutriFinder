using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace NutriFinderClient;

public class NutritionClient
{
    private HttpClient httpClient;

    public NutritionClient()
    {
        
    }
    
    public NutritionClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    
    public string ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Input can not be empty";

        if (input.Any(char.IsDigit))
            return "Input can not contain numbers";
        
        // if (input.Any(char.IsSymbol))
        //     return "Input can not contain special characters";
        //
        // if (!input.Any(char.IsAsciiLetter)) 
        //     return "Input can only be A-Z with no tone indicators";
        
       if (!Regex.IsMatch(input, "^[a-åA-Å ]+$"))
           return "Only English letters is accepted";
       
       return "ok";
    }

    public string FormatNutritionOutput(NutritionDTO? dto)
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
        //using var http = new HttpClient();
        
        var request = $"/api/nutrition?foodItemName={query}";
        var response = await httpClient.GetAsync(request);
        
        Console.WriteLine(response.StatusCode + " with query: " + request);
        
        if (!response.IsSuccessStatusCode) return null; 

        return await response.Content.ReadFromJsonAsync<NutritionDTO>();
    }
}
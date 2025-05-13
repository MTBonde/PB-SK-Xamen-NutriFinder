using System.Text.RegularExpressions;

namespace NutriFinderClient;

public class NutritionClient
{
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
        
       if (!Regex.IsMatch(input, "^[a-zA-Z ]+$"))
           return "Only English letters is accepted";
       
       return "ok";
    }

    public string FormatNutritionOutput(NutritionData data)
    {
        return $"""
                Food: {data.FoodItemName}
                Carbohydrates: {data.Carb} g
                Fiber: {data.Fiber} g
                Net Carbohydrates: {data.Carb - data.Fiber} g
                Protein: {data.Protein} g
                Fat: {data.Fat} g
                Calories: {data.Kcal} kcal
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
}
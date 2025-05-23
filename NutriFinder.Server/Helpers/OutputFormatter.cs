using Nutrifinder.Shared;

namespace NutriFinder.Server.Helpers;

public class OutputFormatter
{
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
}
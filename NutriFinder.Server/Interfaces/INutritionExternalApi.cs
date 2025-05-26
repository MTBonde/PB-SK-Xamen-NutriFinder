using Nutrifinder.Shared;
using NutriFinderClient;

namespace NutriFinder.Server.Interfaces;

/// <summary>
/// Represents a client for interacting with an external nutrition API to fetch nutritional data for food items.
/// </summary>
public interface INutritionExternalApi
{
    /// <summary>
    /// Asynchronously fetches the nutritional data for a specified food item from an external nutrition API.
    /// </summary>
    /// <param name="foodItemName">The name of the food item for which nutritional data is requested.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a <see cref="NutritionDTO"/> object with the nutritional details of the food item, or null if data is unavailable.</returns>
    Task<NutritionDTO?> FetchNutritionDataAsync(string foodItemName);
}
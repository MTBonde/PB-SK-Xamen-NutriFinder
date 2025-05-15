using NutriFinderClient;

namespace NutriFinder.Server.Interfaces;

/// <summary>
/// Defines a repository interface for handling nutrition data operations.
/// </summary>
public interface INutritionRepository
{
    /// <summary>
    /// Asynchronously retrieves the nutrition data for a specific food item.
    /// </summary>
    /// <param name="foodItemName">The name of the food item for which nutrition data is requested.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// <see cref="NutritionDTO"/> object with the nutrition details of the specified food item,
    /// or null if no data is found.
    /// </returns>
    Task<NutritionDTO?> GetNutritionDataAsync(string foodItemName);

    /// <summary>
    /// Asynchronously saves the nutrition data for a specified food item into the repository.
    /// </summary>
    /// <param name="dto">The <see cref="NutritionDTO"/> object containing the nutrition details to be stored.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    Task SaveNutrtionDataAsync(NutritionDTO dto);

    /// <summary>
    /// Asynchronously checks if nutrition data exists for a specified food item.
    /// </summary>
    /// <param name="foodItemName">The name of the food item to check for existing nutrition data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a boolean indicating
    /// whether the nutrition data for the specified food item exists.
    /// </returns>
    Task<bool> DoesNutrtionExistAsync(string foodItemName);
}
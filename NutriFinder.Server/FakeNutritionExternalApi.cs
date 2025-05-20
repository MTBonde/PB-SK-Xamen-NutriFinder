using NutriFinder.Server.Interfaces;

namespace NutriFinder.Server
{
    public class FakeNutritionExternalApi : INutritionExternalApi
    {
        public Task<NutritionDTO?> FetchNutritionDataAsync(string foodItemName)
        {
            return Task.FromResult<NutritionDTO?>(new NutritionDTO
            {
                FoodItemName = foodItemName,
                Carb = 100,
                Fiber = 10,
                Protein = 5,
                Fat = 1,
                Kcal = 250,
            });
        }
    }
}
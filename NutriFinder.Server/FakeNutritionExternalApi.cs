using NutriFinder.Server.Interfaces;

namespace NutriFinder.Server
{
    public class FakeNutritionExternalApi : INutritionExternalApi
    {
        public Task<NutritionDTO?> FetchNutritionDataAsync(string foodItemName)
        {
            switch (foodItemName)
            {
                // if 404
                case "Dodo":
                    return Task.FromResult<NutritionDTO?>(null);
                // if 503
                case "TESTFAILEDEXTERNALAPI":
                    throw new Exception("Fake exception");
                    return Task.FromResult<NutritionDTO?>(null);
                default:
                    //200 - OK
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
}
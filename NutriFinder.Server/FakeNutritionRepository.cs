using NutriFinder.Database.Interfaces;
using NutriFinder.Server;
using Nutrifinder.Shared;

namespace NutriFinder.Server
{
    public class FakeNutritionRepository : INutritionRepository
    {
        private Dictionary<string, NutritionDTO> memory = new();
        
        public Task<NutritionDTO?> GetNutritionDataAsync(string foodItemName)
        {
            memory.TryGetValue(foodItemName, out var dto);
            return Task.FromResult(dto);
        }

        public Task SaveNutritionDataAsync(NutritionDTO dto)
        {
            memory[dto.FoodItemName.ToLower()] = dto;
            return Task.CompletedTask;
        }

        public Task<bool> DoesNutritionExistAsync(string foodItemName)
        {
            return Task.FromResult(memory.ContainsKey(foodItemName.ToLower()));
        }
    }
}
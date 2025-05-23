using MongoDB.Driver;
using NutriFinder.Database.Interfaces;
using Nutrifinder.Shared;

namespace NutriFinder.Database
{
    public class MongoNutritionRepository : INutritionRepository
    {
        private IMongoCollection<NutritionDTO> _collection;

        public MongoNutritionRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("NutriFinder");
            _collection = database.GetCollection<NutritionDTO>("Nutrition");
        }

        public Task<NutritionDTO?> GetNutritionDataAsync(string foodItemName)
        {
            throw new NotImplementedException();
        }

        public Task SaveNutritionDataAsync(NutritionDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesNutritionExistAsync(string foodItemName)
        {
            throw new NotImplementedException();
        }
    }
}
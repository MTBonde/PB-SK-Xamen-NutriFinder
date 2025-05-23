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

        public Task<NutritionDTO> GetNutritionDataAsync(string foodItemName)
        {
            Task<NutritionDTO> result = _collection.Find(x => x.FoodItemName == foodItemName).FirstOrDefaultAsync();
            
            Console.WriteLine("has found" + result);

            return result;
        }

        public Task SaveNutritionDataAsync(NutritionDTO dto)
        {
            var result = _collection.InsertOneAsync(dto);
            
            Console.WriteLine("has saved" + result);

            return result;
        }

        public async Task<bool> DoesNutritionExistAsync(string foodItemName)
        {
            var result = _collection.Find(n => n.FoodItemName == foodItemName).AnyAsync();
            Console.WriteLine("does exist" +result);

            return await result;
        }
    }
}
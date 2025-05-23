using MongoDB.Driver;
using NutriFinder.Database.Interfaces;
using Nutrifinder.Shared;

namespace NutriFinder.Database
{
    public class MongoNutritionRepository : INutritionRepository
    {
        private IMongoCollection<NutritionDTO> _collection;

        public MongoNutritionRepository(string connectionString = null, string database = "NutriFinder")
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(database);
            _collection = db.GetCollection<NutritionDTO>("Nutrition");
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
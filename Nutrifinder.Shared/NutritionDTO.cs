using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nutrifinder.Shared
{
    public class NutritionDTO
    {
        // [BsonRepresentation(BsonType.ObjectId)]
        // public string? Id { get; set; }
        
        [BsonId]
        public string FoodItemName { get; set; } = string.Empty;
        public float Carb { get; set; }
        public float Fiber { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Kcal { get; set; }
    }
}
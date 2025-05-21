namespace NutriFinder.Server
{
    public class NutritionDTO
    {
        public string FoodItemName { get; set; } = string.Empty;
        public float Carb { get; set; }
        public float Fiber { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Kcal { get; set; }
    }
}
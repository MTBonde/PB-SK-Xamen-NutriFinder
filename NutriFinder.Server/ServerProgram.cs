namespace NutriFinder.Server
{
    public class ServerProgram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.MapGet("/api/nutrition", (string query) =>
            {
                var response = new
                {
                    FoodItemName = query,
                    Carb = 100,
                    Fiber = 10,
                    Protein = 5,
                    Fat = 1,
                    Kcal = 250,
                };

                return Results.Ok(response);
            });

            app.Run();
        }
    }
}
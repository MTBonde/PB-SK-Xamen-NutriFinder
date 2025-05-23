using Microsoft.AspNetCore.TestHost;
using NutriFinder.Database;
using NutriFinder.Database.Interfaces;
using NutriFinder.Server.Helpers;
using NutriFinder.Server.Interfaces;

namespace NutriFinder.Server
{
    public class ServerProgram : IServerProgram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            const string connectionString = "mongodb://localhost:27017";
            
            
            builder.Services.AddSingleton<INutritionExternalApi, FakeNutritionExternalApi>();
            builder.Services.AddSingleton<INutritionRepository, FakeNutritionRepository>();
            //builder.Services.AddSingleton<INutritionRepository>(new MongoNutritionRepository(connectionString));
            
            builder.Services.AddSingleton<RequestValidator>();
            
            builder.Services.AddControllers();
            
            var app = builder.Build();

            
            
            // security
            app.UseHttpsRedirection();
            app.UseAuthorization();
            
            // Route endpoints
            app.MapControllers();
            
            app.Run();
            

            // 
            //
            // app.MapGet("/api/nutrition", (string query) =>
            // {
            //     var response = new
            //     {
            //         FoodItemName = query,
            //         Carb = 100,
            //         Fiber = 10,
            //         Protein = 5,
            //         Fat = 1,
            //         Kcal = 250,
            //     };
            //
            //     return Results.Ok(response);
            // });

            
        }
    }
}
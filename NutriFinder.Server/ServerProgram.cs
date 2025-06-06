using NutriFinder.Database;
using NutriFinder.Database.Interfaces;
using NutriFinder.Server.External;
using NutriFinder.Server.Helpers;
using NutriFinder.Server.Interfaces;

namespace NutriFinder.Server
{
    public class ServerProgram : IServerProgram
    {
        public static void Main(string[] args)
        {
            // Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
            // Console.WriteLine("File exists: " + File.Exists("External/Frida_5.3_November2024_Dataset.xlsx"));

            
            const string FRIDAPATH = "External/Frida_5.3_November2024_Dataset.xlsx"; 
            //const string connectionString = "mongodb://localhost:27017";
            
            // Read from environment, fall back to localhost for local dev
            var connectionString = Environment.GetEnvironmentVariable("Mongo__ConnectionString") ?? "mongodb://localhost:27017";

            
            var builder = WebApplication.CreateBuilder(args);
            
            //builder.Services.AddSingleton<INutritionExternalApi, FakeNutritionExternalApi>();
            builder.Services.AddSingleton<IExcelNutritionReader>(new ExcelNutritionReader(FRIDAPATH));
            builder.Services.AddSingleton<INutritionExternalApi, DTUNutritionExternalAPI>();


            //builder.Services.AddSingleton<INutritionRepository, FakeNutritionRepository>();
            builder.Services.AddSingleton<INutritionRepository>(new MongoNutritionRepository(connectionString));
            
            builder.Services.AddSingleton<RequestValidator>();
            
            builder.Services.AddControllers();
            
            builder.WebHost.UseUrls("http://0.0.0.0:5000");
            
            var app = builder.Build();
            
            // security
            //app.UseHttpsRedirection();
            app.UseAuthorization();
            
            // Route endpoints
            app.MapControllers();
            
            app.MapGet("/", () =>
            {
                return Results.Json(new
                {
                    status = "NutriFinder API online",
                    version = "1.0.0"
                });
            });
            
            app.Run();
        }
    }
}
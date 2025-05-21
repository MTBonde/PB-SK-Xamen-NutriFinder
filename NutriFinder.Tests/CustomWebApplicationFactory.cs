using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutriFinder.Server;
using NutriFinder.Server.Interfaces;

namespace NutriFinder.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<ServerProgram>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var binDir = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(binDir, @"..\..\..\..\NutriFinder.Server\"));
            builder.UseContentRoot(projectRoot);

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<INutritionRepository, FakeNutritionRepository>();
            });

            return base.CreateHost(builder);
        }
    }
}
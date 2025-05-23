using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutriFinder.Database.Interfaces;
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
                // Remove real repository
                var repoDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(INutritionRepository));
                if (repoDescriptor != null) services.Remove(repoDescriptor);

                // Remove real external API
                var apiDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(INutritionExternalApi));
                if (apiDescriptor != null) services.Remove(apiDescriptor);

                // Add fakes
                services.AddSingleton<INutritionRepository, FakeNutritionRepository>();
                services.AddSingleton<INutritionExternalApi, FakeNutritionExternalApi>();
            });


            return base.CreateHost(builder);
        }
    }
}
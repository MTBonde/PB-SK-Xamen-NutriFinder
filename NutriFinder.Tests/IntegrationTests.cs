using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NutriFinder.Server;
using NutriFinder.Server.Helpers;

namespace NutriFinder.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private WebApplicationFactory<ServerProgram> factory;
        private HttpClient client;
        
        [TestInitialize]
        public void Setup()
        {
            factory = new WebApplicationFactory<ServerProgram>();
            client = factory.CreateClient();
        }
        
        [TestMethod]
        public void Can_Instantiate_NutritionController()
        {
            // Arrange
            var controller = new NutritionController(null, null, null);
        
            // Act
            
            // Assert
            Assert.IsNotNull(controller);
        }
        
        [TestMethod]
        public void Can_Instantiate_NutritionControllerWithDependencies()
        {
            // Arrange
            var repository = new FakeNutritionRepository();
            var api = new FakeNutritionExternalApi();
            var validator = new RequestValidator();
            var controller = new NutritionController(repository, api, validator);
        
            // Act
            
            // Assert
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public async Task Returns400_OnInvalidInput()
        {
            // Arrange
            var input = "";
            var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();
            
            
            
            // Act
            var result = await client.GetAsync($"/nutrition/{input}");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

            //var dto = await result.Content.ReadFromJsonAsync<NutritionDTO>();
            // Assert.AreEqual(input, dto?.FoodItemName);
            // Assert.AreEqual(250, dto?.Kcal);
            // Assert.AreEqual(10, dto?.Fiber);
    }
}
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
        string url = "/api/nutrition?foodItemName=";
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
        public async Task Returns400_OnEmptyInput()
        {
            // Arrange
            var input = "";
            
            // Act
            var result = await client.GetAsync(url);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [TestMethod]
        public async Task Returns400_OnInputWithNumbers()
        {
            // Arrange
            var input = "Æble1";
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [TestMethod]
        public async Task Returns400_OnInputWithSpecials()
        {
            // Arrange
            var input = "Æble!";
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }
        
        [TestMethod]
        public async Task Returns200_OnValidInput_FakeData()
        {
            // Arrange
            var input = "Æble";
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            
            var dto = await result.Content.ReadFromJsonAsync<NutritionDTO>();
             Assert.AreEqual(input, dto?.FoodItemName);
             Assert.AreEqual(250, dto?.Kcal);
             Assert.AreEqual(10, dto?.Fiber);
        }
        
        [TestMethod]
        public async Task Returns404_WhenNotFoundInDBOrExternalAPI()
        {
            // Arrange
            var input = "Dodo";
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }
        
        [TestMethod]
        public async Task Returns503_WhenNotFoundInDBOrExternalAPIDoesntRespond()
        {
            // Arrange
            var input = "TESTFAILEDEXTERNALAPI";
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
        }

        [TestMethod]
        public async Task Returns405_WhenUsingWrongHttpMethod()
        {
            // Arrange
            var input = "Æble";
            
            // Act
            var result = await client.PostAsync(url + input, null);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, result.StatusCode);
        }

        [TestMethod]
        public async Task Returns400_WhenInputIsTooLong()
        {
            // Arrange
            var input = new string('a', 33); 
            
            // Act
            var result = await client.GetAsync(url + input);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
    
}
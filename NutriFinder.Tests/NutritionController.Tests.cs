using Microsoft.AspNetCore.Mvc;
using NutriFinder.Server;
using NutriFinder.Server.Helpers;
using NutriFinder.Server.Interfaces;
using Moq;

namespace NutriFinder.Tests
{
    [TestClass]
    public class NutritionController_Tests
    {
        private Mock<INutritionRepository> _mockRepository;
        private Mock<INutritionExternalApi> _mockAPI;
        private RequestValidator _requestValidator;
        private NutritionController _controller;

        [TestInitialize]
        public void Setup()
        {
            Console.WriteLine("Setup Complete!");
            _mockRepository = new Mock<INutritionRepository>();
            _mockAPI = new Mock<INutritionExternalApi>();
            _requestValidator = new RequestValidator();
            _controller = new NutritionController(_mockRepository.Object, _mockAPI.Object, _requestValidator);
        }

        [TestMethod]
        public async Task Return400_WhenInputIsInvalid()
        {
            // arrange
            var invalidQuery = "";
            
            // act
            var result = await _controller.Get(invalidQuery);
            
            // assert
            Assert.AreEqual(400, ((BadRequestResult)result)?.StatusCode);
        }
        
        [TestMethod]
        public async Task Return404_WhenInputNotFoundInDatabase()
        {
            // Arrange
            var dto = new NutritionDTO { FoodItemName = "Æble", Kcal = 100 };
            _mockRepository.Setup(r => r.GetNutritionDataAsync("Æble")).ReturnsAsync(dto);
        
            // Act
            var result = await _controller.Get("Banan");
        
            // Assert
            Assert.AreEqual(404, (result as NotFoundResult)?.StatusCode);
        }
        
        [TestMethod]
        public async Task Return503_WhenExternalFails_AndWhenNotFoundInDatabase()
        {
            // Arrange
            // Repository returns null for "Banan" (not found in DB)
            _mockRepository.Setup(r => r.GetNutritionDataAsync("Banan")).ReturnsAsync((NutritionDTO?)null);

            // External API throws for "Banan"
            _mockAPI.Setup(API => API.FetchNutritionDataAsync("Banan")).ThrowsAsync(new Exception());
        
            // Act
            var result = await _controller.Get("Banan");
        
            // Assert
            Assert.AreEqual(503, (result as ObjectResult)?.StatusCode);
        }
        
        [TestMethod]
        public async Task Return200_WhenInputFoundInDatabase()
        {
            // Arrange
            var dto = new NutritionDTO { FoodItemName = "Æble", Kcal = 100 };
            _mockRepository.Setup(r => r.GetNutritionDataAsync("Æble")).ReturnsAsync(dto);
        
            // Act
            var result = await _controller.Get("Æble");
        
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(dto, (result as OkObjectResult)?.Value);
        }
        
        [TestMethod]
        public async Task Return200_WhenExternalReturnData()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetNutritionDataAsync("Banan")).ReturnsAsync((NutritionDTO?)null);
            var dto = new NutritionDTO { FoodItemName = "Æble", Kcal = 100 };
            _mockAPI.Setup(API => API.FetchNutritionDataAsync("Æble")).ReturnsAsync(dto);
        
            // Act
            var result = await _controller.Get("Æble");
        
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(dto, (result as OkObjectResult)?.Value);
        }
    }
}
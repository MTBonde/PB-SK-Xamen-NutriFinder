using Microsoft.AspNetCore.Mvc.Testing;
using NutriFinder.Server;
using NutriFinderClient;
using NutritionDTO = NutriFinderClient.NutritionDTO;

namespace NutriFinder.Tests
{
    [TestClass]
    public class NutritionClientInputTests
    {
        // Arrange
        string expected = "Only English letters is accepted";
    
        // Act
    
        // Assert
        [TestMethod]
        public void Can_Instantiate_NutritionClient()
        {
            // Arrange
            var client = new NutritionClient();
        
            // Act
    
            // Assert
            Assert.IsNotNull(client);
        }
    
        [TestMethod]
        public void ValidateInput_ShouldRejectEmptyInput()
        {
            // Arrange
            var client = new NutritionClient();
            var input = "";
            var expectedEmpty = "Input can not be empty";
        
            // Act
            var result = client.ValidateInput(input);
    
            // Assert
            Assert.AreEqual(result, expectedEmpty);
        }
    
        [TestMethod]
        public void ValidateInput_ShouldRejectNumbersInInput()
        {
            // Arrange
            var client = new NutritionClient();
            var input = "123";
            var expectedNumbers = "Input can not contain numbers";
        
            // Act
            var result = client.ValidateInput(input);
    
            // Assert
            Assert.AreEqual(result, expectedNumbers);
        }
    
        [TestMethod]
        public void ValidateInput_ShouldRejectSpecialCharacters()
        {
            // Arrange
            var client = new NutritionClient();
            var input = "@£$";
            // var expected = "Input can not contain special characters";
        
            // Act
            var result = client.ValidateInput(input);
    
            // Assert
            Assert.AreEqual(result, expected);
        }
    
        //Examples of rejection include æ, ø, å, Ô, ò, etc.
        [TestMethod]
        public void ValidateInput_ShouldRejectNonAZ()
        {
            // Arrange
            var client = new NutritionClient();
            var input = "æble";
            // var expected = "Input can only be A-Z with no tone indicators";
        
            // Act
            var result = client.ValidateInput(input);
    
            // Assert
            Assert.AreEqual(result, expected);
        }
    
        [TestMethod]
        public void Input_ShouldBeValid()
        {
            // Arrange
            var client = new NutritionClient();
            var input = "banana";
            string validExpected = "ok";
        
            // Act
            var result = client.ValidateInput(input);
    
            // Assert
            Assert.AreEqual(result, validExpected);
        }
    }

    [TestClass]
    public class NutritionClientOutputTests
    {
        string input = "Æble";
        
        [TestMethod]
        public void Can_Instantiate_NutritionDTO()
        {
            // Arrange
            var dto = new NutritionDTO();
        
            // Act
    
            // Assert
            Assert.IsNotNull(dto);
        }
    
        [TestMethod]
        public void TestOutput_MinimalDTO()
        {
            // Arrange
            var client = new NutritionClient();
            var dto = new NutritionDTO
            {
                FoodItemName = "test",
                Carb = 0,
                Fiber = 0,
                Protein = 0,
                Fat = 0,
                Kcal = 0,
            };
        
            // Act
            var output = client.FormatNutritionOutput(dto);
    
            // Assert
            Assert.IsNotNull(output);
        }
    
        [TestMethod]
        public void TestOutput_ShouldFormatCorrectly()
        {
            // Arrange
            var client = new NutritionClient();
            var dto = new NutritionDTO
            {
                FoodItemName = input,
                Carb = 100,
                Fiber = 10,
                Protein = 5,
                Fat = 1,
                Kcal = 250,
            };
        
            // Act
            var output = client.FormatNutritionOutput(dto);
    
            // Assert
            StringAssert.Contains(output, $"Food: {input}");
            StringAssert.Contains(output, "Calories: 250 kcal");
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn404()
        {
            // Arrange
            var client = new NutritionClient();
            var expectedStatusCode = 404;

            // Act
            var result = client.FormatErrorMessageFromStatusCode(expectedStatusCode);

            // Assert
            Assert.AreEqual("Error: Food item not found", result);
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn200()
        {
            // Arrange
            var client = new NutritionClient();
            var expectedStatusCode = 200;

            // Act
            var result = client.FormatErrorMessageFromStatusCode(expectedStatusCode);

            // Assert
            Assert.AreEqual("Success: OK!", result);
        }

        [TestMethod]
        public void TestErrorCodes_ShouldReturn400()
        {
            // Arrange
            var client = new NutritionClient();
            var expectedStatusCode = 400;
        
            // Act
            var result = client.FormatErrorMessageFromStatusCode(expectedStatusCode);
        
            // Assert
            Assert.AreEqual("Error: Bad request!", result);
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn503()
        {
            // Arrange
            var client = new NutritionClient();
            var expectedStatusCode = 503;
        
            // Act
            var result = client.FormatErrorMessageFromStatusCode(expectedStatusCode);
        
            // Assert
            Assert.AreEqual("Error: External API is not available and no cached data was found.", result);
        }
    }

    [TestClass]
    public class FetchNutritionTests
    {
        string input = "Banan";

        private WebApplicationFactory<ServerProgram>? factory;
        private HttpClient httpClient;

        [TestInitialize]
        public void Setup()
        {
            factory = new WebApplicationFactory<ServerProgram>();
            httpClient = factory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public async Task FetchNutritionDataAsync_isNotNull()
        {
            // Arrange
            var client = new NutritionClient(httpClient);
        
            // Act
            var result = await client.FetchNutritionDataAsync(input);
        
            // Assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public async Task FetchNutritionDataAsync_ReturnsExpectedResultWhenServerIsRunning()
        {
            // Arrange
            var client = new NutritionClient(httpClient);
        
            // Act
            var result = await client.FetchNutritionDataAsync(input);
        
            // Assert
            Assert.AreEqual(input, result?.FoodItemName);
            Assert.AreEqual(250, result?.Kcal);
        }
    }
}
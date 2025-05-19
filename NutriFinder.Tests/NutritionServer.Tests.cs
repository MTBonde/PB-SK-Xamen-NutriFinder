using NutriFinderClient;

namespace NutriFinder.Tests;

public class NutritionServer_Tests
{
    [TestClass]
    public class NutritionServerRequestTests
    {
        // Arrange
        string expected = "Only English letters is accepted";
    
        // Act
    
        // Assert
        
        [TestMethod]
        public void Can_Instantiate_NutritionServer()
        {
            // Arrange
            // var client = new NutritionServer();
        
            // Act
    
            // Assert
            // Assert.IsNotNull(client);
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectEmptyRequest()
        {
            // Arrange
            // var client = new NutritionClient();
            var Request = "";
            var expectedEmpty = "Request can not be empty";
        
            // Act
            // var result = client.ValidateRequest(Request);
    
            // Assert
            // Assert.AreEqual(result, expectedEmpty);
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectNumbersInRequest()
        {
            // Arrange
            // var client = new NutritionClient();
            var Request = "123";
            var expectedNumbers = "Request can not contain numbers";
        
            // Act
            // var result = client.ValidateRequest(Request);
    
            // Assert
            // Assert.AreEqual(result, expectedNumbers);
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectSpecialCharacters()
        {
            // Arrange
            // var client = new NutritionClient();
            var Request = "@£$";
            // var expected = "Request can not contain special characters";
        
            // Act
            // var result = client.ValidateRequest(Request);
    
            // Assert
            // Assert.AreEqual(result, expected);
        }
    
        //Examples of rejection include æ, ø, å, Ô, ò, etc.
        [TestMethod]
        public void ValidateRequest_ShouldRejectNonAZ()
        {
            // Arrange
            // var client = new NutritionClient();
            var Request = "æble";
            // var expected = "Request can only be A-Z with no tone indicators";
        
            // Act
            // var result = client.ValidateRequest(Request);
    
            // Assert
            // Assert.AreEqual(result, expected);
        }
    
        [TestMethod]
        public void Request_ShouldBeValid()
        {
            // Arrange
            // var client = new NutritionClient();
            var Request = "banana";
            string validExpected = "ok";
        
            // Act
            // var result = client.ValidateRequest(Request);
    
            // Assert
            // Assert.AreEqual(result, validExpected);
        }
    }
    
    [TestClass]
    public class NutritionServerOutputTests
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
}
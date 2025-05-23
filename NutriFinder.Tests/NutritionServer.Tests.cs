using NutriFinder.Server;
using NutriFinder.Server.Helpers;
using Nutrifinder.Shared;

namespace NutriFinder.Tests
{
    [TestClass]
    public class RequestValidationTests
    {
        // Arrange
        string expected = "Only English letters is accepted";
    
        // Act
    
        // Assert
        
        private RequestValidator validator;
        
        [TestInitialize]
        public void Setup()
        {
            validator = new RequestValidator();
        }
        
        [TestMethod]
        public void Can_Instantiate_NutritionServer()
        {
            // Arrange
        
            // Act
    
            // Assert
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectEmptyRequest()
        {
            // Arrange
            var Request = "";
            var expectedEmpty = "Request can not be empty";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, expectedEmpty);
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectNumbersInRequest()
        {
            // Arrange
            var Request = "123";
            var expectedNumbers = "Request can not contain numbers";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, expectedNumbers);
        }
    
        [TestMethod]
        public void ValidateRequest_ShouldRejectSpecialCharacters()
        {
            // Arrange
            var Request = "@£$";
            var expectedSpecial = "Request can not contain special characters";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, expectedSpecial);
        }
        
        [TestMethod]
        public void ValidateRequest_ShouldRejectChineseCharacters()
        {
            // Arrange
            var Request = "鸡肉";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, expected);
        }
    
        //Examples of rejection include Ô, ò, etc.
        [TestMethod]
        public void ValidateRequest_ShouldRejectNonAZ()
        {
            // Arrange
            var Request = "æble";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, expected);
        }
    
        [TestMethod]
        public void Request_ShouldBeValid()
        {
            // Arrange
            var Request = "banana";
            string validExpected = "ok";
        
            // Act
            var result = validator.Validate(Request);
    
            // Assert
            Assert.AreEqual(result, validExpected);
        }
    }
    
    [TestClass]
    public class OutputFromFormaterTests
    {
        
        // Arrange
        string expected = "Only English letters is accepted";
        string input = "Æble";
        OutputFormatter formatter = new OutputFormatter();
        
        // Act
    
        // Assert
        
        [TestInitialize]
        public void Setup()
        {
            formatter = new OutputFormatter();
        }
        
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
            var output = formatter.FormatNutritionOutput(dto);
    
            // Assert
            Assert.IsNotNull(output);
        }
    
        [TestMethod]
        public void TestOutput_ShouldFormatCorrectly()
        {
            // Arrange
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
            var output = formatter.FormatNutritionOutput(dto);
    
            // Assert
            StringAssert.Contains(output, $"Food: {input}");
            StringAssert.Contains(output, "Calories: 250 kcal");
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn404()
        {
            // Arrange
            var expectedStatusCode = 404;

            // Act
            var result = formatter.FormatErrorMessageFromStatusCode(expectedStatusCode);

            // Assert
            Assert.AreEqual("Error: Food item not found", result);
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn200()
        {
            // Arrange
            var expectedStatusCode = 200;

            // Act
            var result = formatter.FormatErrorMessageFromStatusCode(expectedStatusCode);

            // Assert
            Assert.AreEqual("Success: OK!", result);
        }

        [TestMethod]
        public void TestErrorCodes_ShouldReturn400()
        {
            // Arrange
            var expectedStatusCode = 400;
        
            // Act
            var result = formatter.FormatErrorMessageFromStatusCode(expectedStatusCode);
        
            // Assert
            Assert.AreEqual("Error: Bad request!", result);
        }
    
        [TestMethod]
        public void TestErrorCodes_ShouldReturn503()
        {
            // Arrange
            var expectedStatusCode = 503;
        
            // Act
            var result = formatter.FormatErrorMessageFromStatusCode(expectedStatusCode);
        
            // Assert
            Assert.AreEqual("Error: External API is not available and no cached data was found.", result);
        }
    }
}

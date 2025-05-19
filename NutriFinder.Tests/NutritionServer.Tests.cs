namespace NutriFinder.Tests;

public class NutritionServer_Tests
{
    [TestClass]
    public class NutritionClientRequestTests
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
}
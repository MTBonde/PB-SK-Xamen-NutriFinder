using MongoDB.Driver;
using Moq;
using NutriFinder.Database;
using Nutrifinder.Shared;

namespace NutriFinder.Tests
{
    //TODO: Can Read
    //TODO: Can Write
    //TODO: Can Find

    [TestClass]
    public class NutritionDBUnitTests
    {

        [TestMethod]
        public async Task Can_Instantiate_NutritionDB()
        {
            //Arrage
            var mockCollection = new Mock<IMongoCollection<NutritionDTO>>();
            var repo = new MongoNutritionRepository();

            //Act

            //Assert
            Assert.IsNotNull(repo);
        }
    }

    [TestClass]
    public class NutritionDBIntegrationTests()
    {
        private MongoNutritionRepository _repo;
        private NutritionDTO bananDTO;
        
        [TestInitialize]
        public void Setup()
        {
            _repo = new MongoNutritionRepository();
            
            bananDTO = new NutritionDTO { FoodItemName = "Banan", Kcal = 100 };
        }
         
        [TestMethod]
        public async Task Can_Find_NutritionInDB()
        {
            //Arrange
            await _repo.SaveNutritionDataAsync(bananDTO);
            
            //Act
            var doesNutritionExist = await _repo.DoesNutritionExistAsync("Banan");
            
            //Assert
            Assert.IsTrue(doesNutritionExist);
        }

        [TestMethod]
        public async Task Can_Read_NutritionDB()
        {
            //Arrange
            
            //Act
            
            //Assert
        }
        
        [TestMethod]
        public async Task Can_Write_NutritionDB()
        {
            //Arrange
            
            //Act
            
            //Assert
        }
    }
}
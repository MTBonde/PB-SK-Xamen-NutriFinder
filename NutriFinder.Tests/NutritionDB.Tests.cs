using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MongoDB.Driver;
using Moq;
using NutriFinder.Database;
using NutriFinder.Database.Interfaces;
using NutriFinder.Server;
using Nutrifinder.Shared;

namespace NutriFinder.Tests
{
    [TestClass]
    public class NutritionRepositoryContractTests
    {
        [TestMethod]
        public async Task FakeRepository_ShouldFulfill_INutritionRepository_Contract()
        {
            INutritionRepository repo = new FakeNutritionRepository();
            var dto = new NutritionDTO { FoodItemName = "Pear", Kcal = 88 };

            await repo.SaveNutritionDataAsync(dto);

            var exists = await repo.DoesNutritionExistAsync("Pear");
            Assert.IsTrue(exists);

            var loaded = await repo.GetNutritionDataAsync("Pear");
            Assert.IsNotNull(loaded);
            Assert.AreEqual(dto.Kcal, loaded.Kcal);
        }
    }
    [TestClass]
    public class NutritionDBUnitTests
    {

        [TestMethod]
        public async Task Can_Instantiate_NutritionDB()
        {
            //Arrage
            var mockCollection = new Mock<IMongoCollection<NutritionDTO>>();
            var repo = new MongoNutritionRepository("mongodb://localhost:42069");

            //Act

            //Assert
            Assert.IsNotNull(repo);
        }
    }

    [TestClass]
    public class NutritionDBIntegrationTests() : IDisposable
    {
        private IContainer container;
        private MongoNutritionRepository _repo;
        private NutritionDTO bananDTO;
        private string connectionString;
        
        [TestInitialize]
        public void Setup()
        {
            container = new ContainerBuilder()
                .WithImage("mongo:7.0.21")
                .WithPortBinding(27017, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(27017))
                .Build();
            
            container.StartAsync().Wait();
            
            var hostPort = container.GetMappedPublicPort(27017);
            connectionString = $"mongodb://localhost:{hostPort}";
            
            _repo = new MongoNutritionRepository(connectionString, "testdb");
            
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
            var input = "Banan";
            await _repo.SaveNutritionDataAsync(bananDTO);

            //Act
            NutritionDTO returnResult = await _repo.GetNutritionDataAsync(input);

            //Assert
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(bananDTO.FoodItemName, returnResult.FoodItemName);
        }
        
        [TestMethod]
        public async Task Can_Write_NutritionDB()
        {
            //Arrange
            var input = "Banan";
            await _repo.SaveNutritionDataAsync(bananDTO);

            //Act
            var doesExist = await _repo.DoesNutritionExistAsync(input);

            //Assert
            Assert.IsTrue(doesExist);
        }

        public void Dispose()
        {
            container.StopAsync();
        }
    }
}
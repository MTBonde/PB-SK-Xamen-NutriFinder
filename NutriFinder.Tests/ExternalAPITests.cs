using ClosedXML.Excel;
using Moq;
using NutriFinder.Server.External;
using Nutrifinder.Shared;

namespace NutriFinder.Tests
{
    [TestClass]
    public class ExternalAPITests
    {
        [TestMethod]
        public async Task FetchNutritionDataAsync_ReturnsDTO_WhenReaderReturnsResult() // happy paht
        {
            // Arrange
            var mockReader = new Mock<IExcelNutritionReader>();
            var expected = new NutritionDTO
            {
                FoodItemName = "apple",
                Carb = 10,
                Fiber = 2,
                Protein = 1,
                Fat = 0.5f,
                Kcal = 50
            };
            mockReader.Setup(r => r.Lookup("apple")).Returns(expected);

            var api = new DTUNutritionExternalAPI(mockReader.Object);

            // Act
            var result = await api.FetchNutritionDataAsync("apple");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.FoodItemName, result.FoodItemName);
        }

        [TestMethod]
        public async Task FetchNutritionDataAsync_ReturnsNull_WhenReaderReturnsNull()
        {
            // Arrange
            var mockReader = new Mock<IExcelNutritionReader>();
            mockReader.Setup(r => r.Lookup(It.IsAny<string>())).Returns((NutritionDTO?)null);

            var api = new DTUNutritionExternalAPI(mockReader.Object);

            // Act
            var result = await api.FetchNutritionDataAsync("banana");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FetchNutritionDataAsync_ReturnsNull_WhenInputIsWhitespace()
        {
            // Arrange
            var mockReader = new Mock<IExcelNutritionReader>();
            var api = new DTUNutritionExternalAPI(mockReader.Object);

            // Act
            var result = await api.FetchNutritionDataAsync(" ");

            // Assert
            Assert.IsNull(result);
        }
    }
    
     [TestClass]
    public class ExcelNutritionReaderTests
    {
        private string _filePath;

        [TestInitialize]
        public void Setup()
        {
            _filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");
            using var workbook = new XLWorkbook();

            // Create Food sheet
            var foodSheet = workbook.AddWorksheet("Food");
            foodSheet.Cell(1, 1).Value = "FoodID";
            foodSheet.Cell(1, 2).Value = "FoodName";
            foodSheet.Cell(2, 1).Value = 1;
            foodSheet.Cell(2, 2).Value = "Apple";

            // Create Data_Normalised sheet
            var dataSheet = workbook.AddWorksheet("Data_Normalised");
            dataSheet.Cell(1, 1).Value = "FoodID";
            dataSheet.Cell(1, 2).Value = "ParameterName";
            dataSheet.Cell(1, 3).Value = "ResVal";

            dataSheet.Cell(2, 1).Value = 1;
            dataSheet.Cell(2, 2).Value = "Carbohydrate, available";
            dataSheet.Cell(2, 3).Value = 12;

            dataSheet.Cell(3, 1).Value = 1;
            dataSheet.Cell(3, 2).Value = "Dietary fibre";
            dataSheet.Cell(3, 3).Value = 2;

            dataSheet.Cell(4, 1).Value = 1;
            dataSheet.Cell(4, 2).Value = "Protein";
            dataSheet.Cell(4, 3).Value = 0.5;

            dataSheet.Cell(5, 1).Value = 1;
            dataSheet.Cell(5, 2).Value = "Fat";
            dataSheet.Cell(5, 3).Value = 0.2;

            dataSheet.Cell(6, 1).Value = 1;
            dataSheet.Cell(6, 2).Value = "Energy (kcal)";
            dataSheet.Cell(6, 3).Value = 52;

            workbook.SaveAs(_filePath);
        }
        
        [TestMethod]
        public void Lookup_ReturnsNull_WhenSheetIsMissing()
        {
            var reader = new ExcelNutritionReader(_filePath);
            var result = reader.Lookup("Anything");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Lookup_ReturnsCorrectDto_WhenDataIsValid()
        {
            var reader = new ExcelNutritionReader(_filePath);
            var result = reader.Lookup("Apple");

            Assert.IsNotNull(result);
            Assert.AreEqual("Apple", result.FoodItemName);
            Assert.AreEqual(12, result.Carb);
            Assert.AreEqual(2, result.Fiber);
            Assert.AreEqual(0.5f, result.Protein);
            Assert.AreEqual(0.2f, result.Fat);
            Assert.AreEqual(52, result.Kcal);
        }

        [TestMethod]
        public void Lookup_ReturnsNull_WhenFoodNotFound()
        {
            var reader = new ExcelNutritionReader(_filePath);
            var result = reader.Lookup("Banana");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Lookup_SetsMissingFieldsToZero()
        {
            using var workbook = new XLWorkbook();
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");


            var foodSheet = workbook.AddWorksheet("Food");
            foodSheet.Cell(1, 1).Value = "FoodID";
            foodSheet.Cell(1, 2).Value = "FoodName";
            foodSheet.Cell(2, 1).Value = 2;
            foodSheet.Cell(2, 2).Value = "Orange";

            var dataSheet = workbook.AddWorksheet("Data_Normalised");
            dataSheet.Cell(1, 1).Value = "FoodID";
            dataSheet.Cell(1, 2).Value = "ParameterName";
            dataSheet.Cell(1, 3).Value = "ResVal";

            // Only one nutrient provided
            dataSheet.Cell(2, 1).Value = 2;
            dataSheet.Cell(2, 2).Value = "Energy (kcal)";
            dataSheet.Cell(2, 3).Value = 45;

            workbook.SaveAs(path);

            var reader = new ExcelNutritionReader(path);
            var result = reader.Lookup("Orange");

            Assert.IsNotNull(result);
            Assert.AreEqual("Orange", result.FoodItemName);
            Assert.AreEqual(0, result.Carb);
            Assert.AreEqual(0, result.Fiber);
            Assert.AreEqual(0, result.Protein);
            Assert.AreEqual(0, result.Fat);
            Assert.AreEqual(45, result.Kcal);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
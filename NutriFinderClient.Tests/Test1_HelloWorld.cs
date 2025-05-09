namespace NutriFinderClient.Tests;

[TestClass]
public sealed class Test1_HelloWorld
{
    [TestMethod]
    public void GetGreeting_ReturnsHelloWorld()
    {
        string result = HelloWorld.GetGreeting();
        Assert.AreEqual("Hello, World!", result);
    }
}

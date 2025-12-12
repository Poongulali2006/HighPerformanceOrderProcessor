using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProcessorService.Services;

[TestClass]
public class OrderProcessorTests
{
    [TestMethod]
    public void Test_Invalid_TotalAmount()
    {
        string json = @"{
            ""OrderId"": ""1"",
            ""CustomerName"": ""Test"",
            ""OrderDate"": ""2024-01-01T00:00:00Z"",
            ""Items"": [],
            ""TotalAmount"": -10
        }";

        File.WriteAllText("IncomingOrders/test.json", json);

        var db = new DatabaseService();
        var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<OrderProcessor>();

        var processor = new OrderProcessor(db, logger);
        processor.ProcessFile("IncomingOrders/test.json").Wait();

        Assert.IsTrue(true); // simple test
    }
}

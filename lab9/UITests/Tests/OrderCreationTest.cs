using OpenQA.Selenium;
using UITests.ActionMethods;
using UITests.Configurations.OrderCreation;
using UITests.Utilities;

namespace UITests.Tests;

[TestClass]
public class OrderCreationTests
{
    private OrderCreationMethods _orderCreationMethods;
    private IWebDriver _webDriver;
    private const string Url = "http://shop.qatl.ru";

    private const string ValidTestDataFilePath = @"Configurations\OrderCreation\order_creation_valid_data.json";
    private const string InvalidTestDataFilePath = @"Configurations\OrderCreation\order_creation_invalid_data.json";

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(Url);

        _orderCreationMethods = new OrderCreationMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
        _webDriver.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(GetValidTestData), DynamicDataSourceType.Method)]
    public void CreateOrderWithValidData(
        string productLink,
        string quantity,
        string login,
        string password,
        string email,
        string customerName,
        string address,
        string note,
        string alert)
    {
        _webDriver.Navigate().GoToUrl($"{Url}{productLink}");

        _orderCreationMethods.SetCurrentElement(_orderCreationMethods.GetSimpleCartElement());
        _orderCreationMethods.AddProductToCart(int.Parse(quantity));

        _orderCreationMethods.SwitchToCartPage();

        var customerInfo =
            new CustomerInfo($"{login}{DateTime.Now:hh-mm-ss}",
                password,
                customerName,
                $"{DateTime.Now:hh-mm-ss}{email}",
                address,
                note);

        _orderCreationMethods.SubmitCustomerInfo(customerInfo);

        Assert.AreEqual(alert, _orderCreationMethods.GetOrderHeaderText(), "Alert messages are not equal");
    }

    [TestMethod]
    [DynamicData(nameof(GetInvalidTestData), DynamicDataSourceType.Method)]
    public void CreateOrderWithInvalidData(
        string productLink,
        string quantity,
        string login,
        string password,
        string email,
        string customerName,
        string address,
        string note,
        string alert)
    {
        _webDriver.Navigate().GoToUrl($"{Url + productLink}");

        _orderCreationMethods.SetCurrentElement(_orderCreationMethods.GetSimpleCartElement());
        _orderCreationMethods.AddProductToCart(int.Parse(quantity));

        _orderCreationMethods.SwitchToCartPage();

        var customerInfo =
            new CustomerInfo(login, password, customerName, email, address, note);

        _orderCreationMethods.SubmitCustomerInfo(customerInfo);

        Assert.AreEqual(alert, _orderCreationMethods.GetOrderAlertText(), "Alert messages are not equal");
    }

    private static IEnumerable<object[]> GetValidTestData()
    {
        return GetTestData(ValidTestDataFilePath);
    }

    private static IEnumerable<object[]> GetInvalidTestData()
    {
        return GetTestData(InvalidTestDataFilePath);
    }

    private static IEnumerable<object[]> GetTestData(string filePath)
    {
        return JsonDataConverter.Get<OrderCreationTestData>(filePath, x =>
        [
            x.ProductLink,
            x.Quantity,
            x.Login,
            x.Password,
            x.Email,
            x.CustomerName,
            x.Address,
            x.Note,
            x.Alert
        ]);
    }
}
using OpenQA.Selenium;
using UITests.ActionMethods;
using UITests.Configurations.AddingProductToCart;
using UITests.Utilities;

namespace UITests.Tests;

public static class CustomCartAssert
{
    public static void IsSameCartProductInfo(this Assert assert, CartProductInfo expected, CartProductInfo actual)
    {
        Assert.AreEqual(expected.Name.ToLower(), actual.Name.ToLower(), "Expected to get same product names");
        Assert.AreEqual(expected.Quantity, string.IsNullOrEmpty(actual.Quantity) ? "0" : actual.Quantity,
            "Expected to get same product quantities");
        Assert.AreEqual(expected.Price, actual.Price, "Expected to get same product prices");
    }

    public static void IsSameCartTotalInfo(this Assert assert, CartTotalInfo expected, CartTotalInfo actual)
    {
        Assert.AreEqual(expected.Quantity, string.IsNullOrEmpty(actual.Quantity) ? "0" : actual.Quantity,
            "Expected to get same cart quantities");
        Assert.AreEqual(expected.Price, actual.Price, "Expected to get same cart prices");
    }
}

[TestClass]
public class AddProductToCartTests
{
    private AddProductToCartMethods _addToCartMethods;
    private IWebDriver _webDriver;
    private const string Url = "http://shop.qatl.ru";
    
    private const string TestDataFilePath = @"Configurations\AddingProductToCart\adding_product_to_cart_data.json";

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(Url);

        _addToCartMethods = new AddProductToCartMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
        _webDriver.Dispose();
    }

    [DataTestMethod]
    [DynamicData(nameof(AddingProductToCartData))]
    public void AddProductToCart(string productLink,
        string quantity,
        string name,
        string price,
        string totalPrice,
        string totalQuantity)
    {
        _webDriver.Navigate().GoToUrl($"{Url}{productLink}");

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetSimpleCartElement());
        _addToCartMethods.AddProductToCart(int.Parse(quantity));

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetModalElement());

        Assert.That.IsSameCartProductInfo(new CartProductInfo(name, quantity, price),
            _addToCartMethods.GetCartProduct(0));

        Assert.That.IsSameCartTotalInfo(new CartTotalInfo(totalQuantity, totalPrice),
            _addToCartMethods.GetCartTotalInfo());

        var actualSimpleCartTotal = _addToCartMethods.GetSimpleCartTotal();
        Assert.AreEqual(totalPrice, actualSimpleCartTotal,
            $"Simple cart total does not match expected total.\nExpected: {totalPrice}\nActual: {actualSimpleCartTotal}");
    }
    
    public static IEnumerable<object[]> AddingProductToCartData =>
        JsonDataConverter.Get<AddingProductToCartData>(TestDataFilePath, x => 
            [
                x.ProductLink,
                x.Quantity,
                x.Name,
                x.Price,
                x.TotalPrice,
                x.TotalQuantity
            ]);
}
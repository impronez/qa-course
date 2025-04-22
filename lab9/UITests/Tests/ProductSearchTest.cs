using OpenQA.Selenium;
using System.Collections.ObjectModel;
using UITests.ActionMethods;
using UITests.Configurations.ProductSearching;
using UITests.Utilities;

namespace UITests.Tests;

public static class CustomWebElementAssert
{
    public static void IsEachWebElementEnabled(this Assert assert, ReadOnlyCollection<IWebElement> webElements)
    {
        foreach (var element in webElements)
        {
            Assert.IsTrue(element.Enabled, "Expected to see enabled web element");
        }
    }

    public static void IsAppropriateBreadCrumb(this Assert assert, string breadCrumb, List<string> expectedElements)
    {
        foreach (var element in expectedElements)
        {
            Assert.IsTrue(breadCrumb.Contains(element), "Expected to contain each element in bread crumb");
        }
    }
}

[TestClass]
public class ProductSearchTest
{
    private ProductSearchMethods _searchMethods;
    private IWebDriver _webDriver;
    private const string Url = "http://shop.qatl.ru/";
    private const string SearchUrl = "http://shop.qatl.ru/search/";
    
    private const string TestDataFilePath = @"Configurations\ProductSearching\product_searching_data.json";

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(Url);

        _searchMethods = new ProductSearchMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
        _webDriver.Dispose();
    }

    [DataTestMethod]
    [DynamicData(nameof(ProductSearchData))]
    public void SearchProducts(string searchQuery, string count)
    {
        _searchMethods.SetCurrentElement(_searchMethods.GetSearchFormElement());
        var searchMenu = _searchMethods.GetSearchMenuElement();
        Assert.IsFalse(searchMenu.Displayed, "Expected not to see search menu when nothing was given to search");

        _searchMethods.SetCurrentElement(_searchMethods.GetSearchInputElement());
        _searchMethods.FillInputElement(searchQuery);
        Assert.IsTrue(searchMenu.Displayed, "Expected to see search menu when text was given to search");

        var expectedSuggestionsCount = int.Parse(count);
        _searchMethods.SetCurrentElement(searchMenu);
        Assert.AreEqual(expectedSuggestionsCount, _searchMethods.GetSearchMenuSuggesstionsCount(),
            $"Expected to get {expectedSuggestionsCount} number of suggestions");

        _searchMethods.SubmitSearchInput();
        Assert.AreEqual(SearchUrl, $"{_webDriver.Url.Split('?').First()}/",
            "Expected to switch to search result page");
        Assert.That.IsAppropriateBreadCrumb(_searchMethods.GetBreadCrumbText(), [searchQuery]);
        Assert.That.IsEachWebElementEnabled(_searchMethods.GetSearchResults());
    }
    
    public static IEnumerable<object[]> ProductSearchData =>
        JsonDataConverter.Get<ProductSearchTestData>(TestDataFilePath, x => [x.SearchQuery, x.Quantity]);
}
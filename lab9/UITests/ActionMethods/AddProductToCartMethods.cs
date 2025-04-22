using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace UITests.ActionMethods;

public struct CartProductInfo
{
    public CartProductInfo(string name, string quantity, string price)
    {
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public string Name;
    public string Quantity;
    public string Price;
}

public struct CartTotalInfo
{
    public CartTotalInfo(string quantity, string price)
    {
        Quantity = quantity;
        Price = price;
    }

    public string Quantity;
    public string Price;
}

public class AddProductToCartMethods
{
    private readonly IWebDriver _webDriver;
    private IWebElement _currentElement;

    protected static readonly By _cartAddXPath = By.XPath("//*[contains(@class, 'add-to-cart-link')]"),
        _cartQuantityXPath = By.XPath("//input[@name='quantity']"),
        _cartXPath = By.XPath("//*[contains(@class,'simpleCart_shelfItem')]"),
        _modalXPath = By.XPath("//div[contains(@class,'modal-content')]"),
        _modalProductRowsXPath = By.XPath(".//tr[descendant::a]"),
        _modalTotalXPath = By.XPath(".//tr[descendant::td[contains(@class, 'cart')]]"),
        _cartSimpleTotalXpath = By.XPath("//*[@class='simpleCart_total']");

    public AddProductToCartMethods(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public void SetCurrentElement(IWebElement webElement)
    {
        _currentElement = webElement;
    }

    public IWebElement GetSimpleCartElement()
    {
        return _webDriver.FindElement(_cartXPath);
    }

    public void AddProductToCart(int quantity)
    {
        var quantityElement = _currentElement.FindElement(_cartQuantityXPath);
        quantityElement.Clear();
        quantityElement.SendKeys(quantity.ToString());

        var cartAdd = _currentElement.FindElement(_cartAddXPath);
        cartAdd.Click();

        WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
        wait.Until(e => _webDriver.FindElement(_modalXPath).Displayed);
    }

    public IWebElement GetModalElement()
    {
        return _webDriver.FindElement(_modalXPath);
    }

    public CartProductInfo GetCartProduct(int index)
    {
        var products = _webDriver.FindElements(_modalProductRowsXPath);
        var product = products[index];

        var columns = product.FindElements(By.XPath(".//td"));

        return new CartProductInfo(columns[1].Text, columns[2].Text, columns[3].Text);
    }

    public CartTotalInfo GetCartTotalInfo()
    {
        var total = _webDriver.FindElements(_modalTotalXPath);

        var totalQuantityRow = total.First();
        var totalPriceRow = total.Last();

        var totalQuantityColumns = totalQuantityRow.FindElements(By.XPath(".//td"));
        var totalPriceColumns = totalPriceRow.FindElements(By.XPath(".//td"));

        return new CartTotalInfo(totalQuantityColumns[1].Text, totalPriceColumns[1].Text.Remove(0, 1));
    }

    public string GetSimpleCartTotal()
    {
        return _webDriver.FindElement(_cartSimpleTotalXpath).Text.Remove(0, 1);
    }
}

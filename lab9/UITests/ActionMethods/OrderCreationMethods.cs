using OpenQA.Selenium;

namespace UITests.ActionMethods;

public struct CustomerInfo
{
    public CustomerInfo(string login, string password, string name, string email, string address, string note)
    {
        Login = login;
        Password = password;
        Name = name;
        Email = email;
        Address = address;
        Note = note;
    }

    public string Login;
    public string Password;
    public string Name;
    public string Email;
    public string Address;
    public string Note;
}

public class OrderCreationMethods : AddProductToCartMethods
{
    private readonly IWebDriver _webDriver;
    private static readonly By _formXPath = By.XPath("//form[@action='cart/checkout']"),                     
        _inputLoginXPath = By.XPath(".//input[@id='login']"),
        _inputPasswordXPath = By.XPath(".//input[@id='pasword']"),
        _inputNameXPath = By.XPath(".//input[@id='name']"),
        _inputEmailXPath = By.XPath(".//input[@id='email']"),
        _inputAddressXPath = By.XPath(".//input[@id='address']"),
        _inputNoteXPath = By.XPath(".//textarea[@name='note']"),
        _inputSubmitButtonXPath = By.XPath(".//button[@type='submit']"),
        _modalMakeOrderButtonXPath = By.XPath("//a[@href='cart/view']"),
        _orderAlertXPath = By.XPath("//div[contains(@class,'alert')] "),
        _orderHeaderXPath = By.XPath("//h1");


    public OrderCreationMethods(IWebDriver webDriver) : base(webDriver)
    {
        _webDriver = webDriver;
    }

    public void SwitchToCartPage()
    {
        _webDriver.FindElement(_modalMakeOrderButtonXPath).Click();
    }

    public void SubmitCustomerInfo(CustomerInfo customerInfo)
    {
        var form = _webDriver.FindElement(_formXPath);
        form.FindElement(_inputLoginXPath).SendKeys(customerInfo.Login);
        form.FindElement(_inputPasswordXPath).SendKeys(customerInfo.Password);
        form.FindElement(_inputNameXPath).SendKeys(customerInfo.Name);
        form.FindElement(_inputEmailXPath).SendKeys(customerInfo.Email);
        form.FindElement(_inputAddressXPath).SendKeys(customerInfo.Address);
        form.FindElement(_inputNoteXPath).SendKeys(customerInfo.Note);

        form.FindElement(_inputSubmitButtonXPath).Click();
    }

    public string GetOrderAlertText()
    {
        return _webDriver.FindElement(_orderAlertXPath).Text;
    }

    public string GetOrderHeaderText()
    {
        return _webDriver.FindElement(_orderHeaderXPath).Text;
    }
}
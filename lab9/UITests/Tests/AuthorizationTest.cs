using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UITests.ActionMethods;
using UITests.Configurations.Authorization;
using UITests.Utilities;

namespace UITests.Tests;

[TestClass]
public class AuthorizationTest
{
    private IWebDriver _webDriver;
    private AuthorizationMethods _authorizeMethods;

    private const string Url = "http://shop.qatl.ru";
    private const string LoginUrl = "/user/login";
    private const string TestDataFilePath = @"Configurations\Authorization\authorization_data.json";

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(Url);

        _authorizeMethods = new AuthorizationMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
        _webDriver.Dispose();
    }

    [DataTestMethod]
    [DynamicData(nameof(AuthorizationData))]
    public void Authorize(string login, string password, string expectedMessage)
    {
        var drop = _authorizeMethods.GetDropElement();
        _authorizeMethods.SetCurrentElement(drop);

        var dropdownMenu = _authorizeMethods.GetDropdownMenuElement();
        Assert.IsFalse(dropdownMenu.Displayed, "Login dropdown should not be visible when the page loads.");

        _authorizeMethods.ToggleAccountDropdownElement();
        Assert.IsTrue(dropdownMenu.Displayed,
            "Login dropdown should become visible after toggling the account button.");
        _authorizeMethods.SetCurrentElement(dropdownMenu);

        _authorizeMethods.ClickOnSignInButton();
        Assert.AreEqual($"{Url}{LoginUrl}",
            _webDriver.Url,
            $"Expected to navigate to login page '{Url}{LoginUrl}', but current URL is '{_webDriver.Url}'.");

        _authorizeMethods.SetCurrentElement(_authorizeMethods.GetLoginFormElement());
        _authorizeMethods.SubmitUserInfo(login, password);

        var actualMessage = _authorizeMethods.GetAlertMessageElement().Text;
        Assert.AreEqual(expectedMessage, actualMessage,
            $"Expected alert message: '{expectedMessage}', but got: '{actualMessage}'.");
    }

    public static IEnumerable<object[]> AuthorizationData =>
        JsonDataConverter.Get<AuthorizationTestData>(TestDataFilePath, x => [x.Login, x.Password, x.ExpectedMessage]);
}
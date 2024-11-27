using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using OpenQA.Selenium.Interactions;

namespace BMTUITesting
{
    public class LoginTests
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void VerifyLoginButtonNavigatesToLoginPage()
        {
            _driver.Navigate().GoToUrl("http://localhost:8080/"); 
            var loginLink = _driver.FindElement(By.CssSelector("a[href='/login']"));
            loginLink.Click();

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("/login"));
            Assert.IsTrue(_driver.Url.Contains("/login"), "No se redirigió correctamente a la página de inicio de sesión.");
        }

        [Test]
        public void LoginWithValidCredentials()
        {
            _driver.Navigate().GoToUrl("http://localhost:8080/");

            var loginButton = _driver.FindElement(By.CssSelector("a[href='/login']"));
            loginButton.Click();

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("form")));

            var emailInput = _driver.FindElement(By.CssSelector("a[href='/inputCorreo'] input"));
            emailInput.SendKeys("brandon.trigueros@ucr.ac.cr");

            var passwordInput = _driver.FindElement(By.CssSelector("a[href='/inputContrasena'] input"));
            passwordInput.SendKeys("Aa1.");

            var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            var okButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".swal2-confirm")));
            okButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("http://localhost:8080/enterprises"));
            Assert.AreEqual("http://localhost:8080/enterprises", _driver.Url, "La redirección al home no fue exitosa.");
        }


        [Test]
        public void LoginAndAccessProfile()
        {
            _driver.Navigate().GoToUrl("http://localhost:8080/");

            var loginButton = _driver.FindElement(By.CssSelector("a[href='/login']"));
            loginButton.Click();

            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("form")));

            var emailInput = _driver.FindElement(By.CssSelector("a[href='/inputCorreo'] input"));
            emailInput.SendKeys("brandon.trigueros@ucr.ac.cr");

            var passwordInput = _driver.FindElement(By.CssSelector("a[href='/inputContrasena'] input"));
            passwordInput.SendKeys("Aa1.");

            var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
            submitButton.Click();

            var okButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".swal2-confirm")));
            okButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("http://localhost:8080/enterprises"));
            Assert.AreEqual("http://localhost:8080/enterprises", _driver.Url, "La redirección al home no fue exitosa.");

            var sideBar = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".v-navigation-drawer")));
            Actions actions = new Actions(_driver);
            actions.MoveToElement(sideBar).Perform();

            var profileButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("v-list-item[title='Mi Perfil']")));
            profileButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("http://localhost:8080/profile"));
            Assert.AreEqual("http://localhost:8080/profile", _driver.Url, "La redirección a Mi Perfil no fue exitosa.");
        }


        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
        }
    }
}
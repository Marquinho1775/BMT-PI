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
            // Iniciar el navegador Chrome
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void VerifyLoginButtonNavigatesToLoginPage()
        {
            // Navegar a la página principal
            _driver.Navigate().GoToUrl("http://localhost:8080/"); // Cambia por tu URL

            // Localizar el enlace <a> que contiene el botón "Iniciar Sesión"
            var loginLink = _driver.FindElement(By.CssSelector("a[href='/login']"));

            // Hacer clic en el enlace
            loginLink.Click();

            // Validar la redirección a la página de login
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("/login"));
            Assert.IsTrue(_driver.Url.Contains("/login"), "No se redirigió correctamente a la página de inicio de sesión.");
        }

        [Test]
        public void LoginWithValidCredentials()
        {
            // Navegar a la página principal
            _driver.Navigate().GoToUrl("http://localhost:8080/"); // Cambia por la URL de tu aplicación

            // Encontrar y hacer clic en el botón "Iniciar Sesión"
            var loginButton = _driver.FindElement(By.CssSelector("a[href='/login']"));
            loginButton.Click();

            // Esperar a que se cargue la página de login
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("formLogin")));

            // Rellenar el campo de correo
            var emailInput = _driver.FindElement(By.Id("email"));
            emailInput.SendKeys("brandon.trigueros@ucr.ac.cr");

            // Rellenar el campo de contraseña
            var passwordInput = _driver.FindElement(By.Id("password"));
            passwordInput.SendKeys("Aa1.");

            // Hacer clic en el botón "Iniciar sesión"
            var submitButton = _driver.FindElement(By.CssSelector("b-button[type='submit']"));
            submitButton.Click();

            // Validar que se redirige correctamente al home page
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("https://tudominio.com/")); // Cambia por la URL del home
            Assert.AreEqual("https://tudominio.com/", _driver.Url, "La redirección al home no fue exitosa.");
        }

        [Test]
        public void LoginAndAccessProfile()
        {
            // Navegar a la página principal
            _driver.Navigate().GoToUrl("https://tudominio.com"); // Cambia por la URL de tu aplicación

            // Encontrar y hacer clic en el botón "Iniciar Sesión"
            var loginButton = _driver.FindElement(By.CssSelector("a[href='/login']"));
            loginButton.Click();

            // Esperar a que se cargue la página de login
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("formLogin")));

            // Rellenar el campo de correo
            var emailInput = _driver.FindElement(By.Id("email"));
            emailInput.SendKeys("brandon.trigueros@ucr.ac.cr");

            // Rellenar el campo de contraseña
            var passwordInput = _driver.FindElement(By.Id("password"));
            passwordInput.SendKeys("Aa1.");

            // Hacer clic en el botón "Iniciar sesión"
            var submitButton = _driver.FindElement(By.CssSelector("b-button[type='submit']"));
            submitButton.Click();

            // Validar que se redirige correctamente al home page
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("https://tudominio.com/")); // Cambia por la URL del home
            Assert.AreEqual("https://tudominio.com/", _driver.Url, "La redirección al home no fue exitosa.");

            // Hover sobre la side bar para abrirla
            var sideBar = _driver.FindElement(By.CssSelector("v-navigation-drawer"));
            Actions actions = new Actions(_driver);
            actions.MoveToElement(sideBar).Perform();

            // Esperar que el botón "Mi Perfil" sea visible
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//v-list-item[@title='Mi Perfil']")));

            // Hacer clic en el botón "Mi Perfil"
            var profileButton = _driver.FindElement(By.XPath("//v-list-item[@title='Mi Perfil']"));
            profileButton.Click();

            // Validar que se redirige a la página del perfil
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlToBe("https://tudominio.com/profile")); // Cambia por la URL del perfil
            Assert.AreEqual("https://tudominio.com/profile", _driver.Url, "La redirección a Mi Perfil no fue exitosa.");
        }

        [TearDown]
        public void Teardown()
        {
            // Cerrar el navegador después de cada test
            _driver.Quit();
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BMT_backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace UnitTestingBMT
{
    [TestFixture]
    public class TokenServiceTests
    {
        private IConfiguration _configuration;
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            // Crear una implementación simple de IConfiguration
            var settings = new Dictionary<string, string>
            {
                { "Jwt:Key", "fK7QW3tM8nXeG9sZ1jKlB2uY4pSdC6rH" },
                { "Jwt:Issuer", "BusinessTracker" },
                { "Jwt:Audience", "Richi" },
                { "Jwt:ExpireMinutes", "60" } // Ensure this is a string
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings) // Agregar los valores directamente
                .Build();

            // Crear la instancia de TokenService
            _tokenService = new TokenService(_configuration);
        }

        [Test]
        public void GenerateToken_ConUsuarioValido_DeberiaRetornarTokenValido()
        {
            // Arrange
            var user = new UserModel
            {
                Id = "26E5D181-41C9-4729-98AD-429094C9D625",
                Name = "David",
                LastName = "GV",
                Username = "dav",
                Email = "davidgv03@hotmail.com",
                IsVerified = true,
                Password = "Aa1.",
                Role = "cli",
                ProfilePictureURL = "/uploads/default.png"
            };

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsInstanceOf<string>(token);
        }

        [Test]
        public void GenerateToken_MissingConfiguration_ThrowsArgumentException()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Jwt:Key"]).Returns((string)null); // Missing "Jwt:Key"

            var user = new UserModel { Email = "john.doe@example.com" }; // Only provide required user properties
            var tokenService = new TokenService(mockConfiguration.Object);

            // Act & Assert (Expect ArgumentException)
            Assert.Throws<ArgumentException>(() => tokenService.GenerateToken(user));
        }

        [Test]
        public void GenerateToken_ConUsuarioInvalido_DeberiaRetornarNull()
        {
            // Arrange
            var user = new UserModel
            {
                Id = "",
                Name = "David",
                LastName = "GV",
                Username = "dav",
                Email = "",
                IsVerified = true,
                Password = "Aa1.",
                Role = "cli",
                ProfilePictureURL = "/uploads/default.png"
            };

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNull(token);
        }
    }
}

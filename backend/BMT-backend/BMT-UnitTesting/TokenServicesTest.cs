using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using BMT_backend.Models;
using BMT_backend.Services;
using System;

namespace UnitTestingBMT
{
    [TestFixture]
    public class TokenServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private TokenService _tokenService;
        private UserModel _testUser;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("fK7QW3tM8nXeG9sZ1jKlB2uY4pSdC6rH");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("testissuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("testaudience");
            _mockConfiguration.Setup(config => config["Jwt:ExpireMinutes"]).Returns("30");

            _tokenService = new TokenService(_mockConfiguration.Object);

            _testUser = new UserModel
            {
                Id = "1",
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "johndoe@example.com",
                IsVerified = true,
                Password = "password",
                Role = "User",
                ProfilePictureURL = "http://example.com/profile.jpg",
            };
        }

        [Test]
        public void GenerateToken_ValidUser_ReturnsToken()
        {
            // Act
            var token = _tokenService.GenerateToken(_testUser);

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void GenerateToken_NullUser_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _tokenService.GenerateToken(null));
        }

        [Test]
        public void GenerateToken_UserWithoutEmail_ThrowsArgumentException()
        {
            // Arrange
            _testUser.Email = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _tokenService.GenerateToken(_testUser));
        }

        [Test]
        public void GenerateToken_UserWithoutName_ThrowsArgumentException()
        {
            // Arrange
            _testUser.Name = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _tokenService.GenerateToken(_testUser));
        }


        [Test]
        public void GenerateToken_UserWithoutValidName_ThrowsArgumentException()
        {
            // Arrange
            _testUser.Name = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _tokenService.GenerateToken(_testUser));
        }

        [Test]
        public void GenerateToken_UserWithoutValidEmail_ThrowsArgumentException()
        {
            // Arrange
            _testUser.Email = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _tokenService.GenerateToken(_testUser));
        }
    }
}
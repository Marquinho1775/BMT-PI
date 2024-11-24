using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class ShoppingCartServiceTests
    {
        private Mock<IShoppingCartRepository> _shoppingCartRepositoryMock;
        private ShoppingCartService _shoppingCartService;

        [SetUp]
        public void SetUp()
        {
            _shoppingCartRepositoryMock = new Mock<IShoppingCartRepository>();
            _shoppingCartService = new ShoppingCartService(_shoppingCartRepositoryMock.Object);
        }

        [Test]
        public async Task CreateShoppingCartAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var userName = "testUser";
            _shoppingCartRepositoryMock
                .Setup(repo => repo.CreateShoppingCartAsync(userName))
                .ReturnsAsync(true);

            // Act
            var result = await _shoppingCartService.CreateShoppingCartAsync(userName);

            // Assert
            Assert.That(result, Is.True);
            _shoppingCartRepositoryMock.Verify(repo => repo.CreateShoppingCartAsync(userName), Times.Once);
        }

        [Test]
        public async Task GetShoppingCartAsync_ShouldReturnShoppingCart_WhenRepositoryReturnsCart()
        {
            // Arrange
            var userId = "user123";
            var expectedCart = new ShoppingCart { Id = "cart123", UserId = userId };
            _shoppingCartRepositoryMock
                .Setup(repo => repo.GetShoppingCartAsync(userId))
                .ReturnsAsync(expectedCart);

            // Act
            var result = await _shoppingCartService.GetShoppingCartAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCart));
            _shoppingCartRepositoryMock.Verify(repo => repo.GetShoppingCartAsync(userId), Times.Once);
        }
    }
}
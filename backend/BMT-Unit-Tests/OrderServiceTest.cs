/*
using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class OrderServiceTest
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object);
        }

        // Tests for IsDirectionUsedInOrders
        [Test]
        public async Task IsDirectionUsedInOrders_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var directionId = "direction123";
            _orderRepositoryMock
                .Setup(repo => repo.IsDirectionUsedInOrdersAsync(directionId))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.IsDirectionUsedInOrders(directionId);

            // Assert
            Assert.That(result, Is.True);
            _orderRepositoryMock.Verify(repo => repo.IsDirectionUsedInOrdersAsync(directionId), Times.Once);
        }

        [Test]
        public async Task IsDirectionUsedInOrders_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var directionId = "direction123";
            _orderRepositoryMock
                .Setup(repo => repo.IsDirectionUsedInOrdersAsync(directionId))
                .ReturnsAsync(false);

            // Act
            var result = await _orderService.IsDirectionUsedInOrders(directionId);

            // Assert
            Assert.That(result, Is.False);
            _orderRepositoryMock.Verify(repo => repo.IsDirectionUsedInOrdersAsync(directionId), Times.Once);
        }

        [Test]
        public void IsDirectionUsedInOrders_ShouldThrowArgumentException_WhenDirectionIdIsNullOrEmpty()
        {
            // Arrange
            string nullDirectionId = null;
            string emptyDirectionId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.IsDirectionUsedInOrders(nullDirectionId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.IsDirectionUsedInOrders(emptyDirectionId));
        }

        // Tests for IsProductUsedInOrders
        [Test]
        public async Task IsProductUsedInOrders_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var productId = "product123";
            _orderRepositoryMock
                .Setup(repo => repo.IsProductUsedInOrdersAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.IsProductUsedInOrders(productId);

            // Assert
            Assert.That(result, Is.True);
            _orderRepositoryMock.Verify(repo => repo.IsProductUsedInOrdersAsync(productId), Times.Once);
        }

        [Test]
        public async Task IsProductUsedInOrders_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var productId = "product123";
            _orderRepositoryMock
                .Setup(repo => repo.IsProductUsedInOrdersAsync(productId))
                .ReturnsAsync(false);

            // Act
            var result = await _orderService.IsProductUsedInOrders(productId);

            // Assert
            Assert.That(result, Is.False);
            _orderRepositoryMock.Verify(repo => repo.IsProductUsedInOrdersAsync(productId), Times.Once);
        }

        [Test]
        public void IsProductUsedInOrders_ShouldThrowArgumentException_WhenProductIdIsNullOrEmpty()
        {
            // Arrange
            string nullProductId = null;
            string emptyProductId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.IsProductUsedInOrders(nullProductId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.IsProductUsedInOrders(emptyProductId));
        }

        // Tests for AreEnterpriseProductsInOrders
        [Test]
        public async Task AreEnterpriseProductsInOrders_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var enterpriseId = "enterprise123";
            _orderRepositoryMock
                .Setup(repo => repo.AreEnterpriseProductsInOrders(enterpriseId))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.AreEnterpriseProductsInOrders(enterpriseId);

            // Assert
            Assert.That(result, Is.True);
            _orderRepositoryMock.Verify(repo => repo.AreEnterpriseProductsInOrders(enterpriseId), Times.Once);
        }

        [Test]
        public async Task AreEnterpriseProductsInOrders_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var enterpriseId = "enterprise123";
            _orderRepositoryMock
                .Setup(repo => repo.AreEnterpriseProductsInOrders(enterpriseId))
                .ReturnsAsync(false);

            // Act
            var result = await _orderService.AreEnterpriseProductsInOrders(enterpriseId);

            // Assert
            Assert.That(result, Is.False);
            _orderRepositoryMock.Verify(repo => repo.AreEnterpriseProductsInOrders(enterpriseId), Times.Once);
        }

        [Test]
        public void AreEnterpriseProductsInOrders_ShouldThrowArgumentException_WhenEnterpriseIdIsNullOrEmpty()
        {
            // Arrange
            string nullEnterpriseId = null;
            string emptyEnterpriseId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AreEnterpriseProductsInOrders(nullEnterpriseId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _orderService.AreEnterpriseProductsInOrders(emptyEnterpriseId));
        }
    }
}
*/
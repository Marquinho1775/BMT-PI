using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class ProductServiceTest
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Test]
        public void DeleteProductAsync_ShouldThrowArgumentException_WhenProductIdIsNullOrEmpty()
        {
            // Arrange
            string nullProductId = null;
            string emptyProductId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _productService.DeleteProductAsync(nullProductId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _productService.DeleteProductAsync(emptyProductId));
        }

        [Test]
        public async Task DeleteProductAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            string validProductId = "product123";
            _productRepositoryMock
                .Setup(repo => repo.DeleteProductAsync(validProductId))
                .ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProductAsync(validProductId);

            // Assert
            Assert.That(result, Is.True);
            _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(validProductId), Times.Once);
        }

        [Test]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            string validProductId = "product123";
            _productRepositoryMock
                .Setup(repo => repo.DeleteProductAsync(validProductId))
                .ReturnsAsync(false);

            // Act
            var result = await _productService.DeleteProductAsync(validProductId);

            // Assert
            Assert.That(result, Is.False);
            _productRepositoryMock.Verify(repo => repo.DeleteProductAsync(validProductId), Times.Once);
        }
    }
}
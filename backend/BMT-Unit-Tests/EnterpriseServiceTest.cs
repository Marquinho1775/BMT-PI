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
    public class EnterpriseServiceTest
    {
        private Mock<IEnterpriseRepository> _enterpriseRepositoryMock;
        private EnterpriseService _enterpriseService;

        [SetUp]
        public void SetUp()
        {
            _enterpriseRepositoryMock = new Mock<IEnterpriseRepository>();
            _enterpriseService = new EnterpriseService(_enterpriseRepositoryMock.Object);
        }

        [Test]
        public async Task DeleteEnterpriseAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var enterpriseId = "enterprise123";
            _enterpriseRepositoryMock
                .Setup(repo => repo.DeleteEnterpriseAsync(enterpriseId))
                .ReturnsAsync(true);

            // Act
            var result = await _enterpriseService.DeleteEnterpriseAsync(enterpriseId);

            // Assert
            Assert.That(result, Is.True);
            _enterpriseRepositoryMock.Verify(repo => repo.DeleteEnterpriseAsync(enterpriseId), Times.Once);
        }

        [Test]
        public async Task DeleteEnterpriseAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var enterpriseId = "enterprise123";
            _enterpriseRepositoryMock
                .Setup(repo => repo.DeleteEnterpriseAsync(enterpriseId))
                .ReturnsAsync(false);

            // Act
            var result = await _enterpriseService.DeleteEnterpriseAsync(enterpriseId);

            // Assert
            Assert.That(result, Is.False);
            _enterpriseRepositoryMock.Verify(repo => repo.DeleteEnterpriseAsync(enterpriseId), Times.Once);
        }

        [Test]
        public void DeleteEnterpriseAsync_ShouldThrowArgumentException_WhenEnterpriseIdIsNullOrEmpty()
        {
            // Arrange
            string nullEnterpriseId = null;
            string emptyEnterpriseId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _enterpriseService.DeleteEnterpriseAsync(nullEnterpriseId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _enterpriseService.DeleteEnterpriseAsync(emptyEnterpriseId));
        }
    }
}
*/
using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class DirectionServiceTest
    {
        private Mock<IDirectionRepository> _directionRepositoryMock;
        private DirectionService _directionService;

        [SetUp]
        public void SetUp()
        {
            _directionRepositoryMock = new Mock<IDirectionRepository>();
            _directionService = new DirectionService(_directionRepositoryMock.Object);
        }

        [Test]
        public async Task DeleteDirectionAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            var directionId = "direction123";
            _directionRepositoryMock
                .Setup(repo => repo.DeleteDirectionAsync(directionId))
                .ReturnsAsync(true);

            // Act
            var result = await _directionService.DeleteDirectionAsync(directionId);

            // Assert
            Assert.That(result, Is.True);
            _directionRepositoryMock.Verify(repo => repo.DeleteDirectionAsync(directionId), Times.Once);
        }

        [Test]
        public async Task DeleteDirectionAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            var directionId = "direction123";
            _directionRepositoryMock
                .Setup(repo => repo.DeleteDirectionAsync(directionId))
                .ReturnsAsync(false);

            // Act
            var result = await _directionService.DeleteDirectionAsync(directionId);

            // Assert
            Assert.That(result, Is.False);
            _directionRepositoryMock.Verify(repo => repo.DeleteDirectionAsync(directionId), Times.Once);
        }

        [Test]
        public void DeleteDirectionAsync_ShouldThrowArgumentException_WhenDirectionIdIsNullOrEmpty()
        {
            // Arrange
            string nullDirectionId = null;
            string emptyDirectionId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _directionService.DeleteDirectionAsync(nullDirectionId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _directionService.DeleteDirectionAsync(emptyDirectionId));
        }
    }
}
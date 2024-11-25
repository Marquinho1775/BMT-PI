using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object, null); // null si no se necesita ITokenService
        }

        [Test]
        public void DeleteUserAsync_ShouldThrowArgumentException_WhenUserIdIsNullOrEmpty()
        {
            // Arrange
            string nullUserId = null;
            string emptyUserId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.DeleteUserAsync(nullUserId));
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.DeleteUserAsync(emptyUserId));
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            // Arrange
            string validUserId = "user123";
            _userRepositoryMock
                .Setup(repo => repo.DeleteUserAsync(validUserId))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserAsync(validUserId);

            // Assert
            Assert.That(result, Is.True);
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(validUserId), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            // Arrange
            string validUserId = "user123";
            _userRepositoryMock
                .Setup(repo => repo.DeleteUserAsync(validUserId))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteUserAsync(validUserId);

            // Assert
            Assert.That(result, Is.False);
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(validUserId), Times.Once);
        }
    }
}

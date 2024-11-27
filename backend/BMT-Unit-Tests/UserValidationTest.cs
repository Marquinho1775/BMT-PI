using NUnit.Framework;
using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using Moq;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class UserValidationTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object);
        }

        #region Tests for ValidateUserRole

        [Test]
        public async Task UpdateRoleAsync_ShouldReturnTrue_WhenRoleIsValid()
        {
            // Arrange
            string userId = "user123";
            string role = "dev";
            _userRepositoryMock.Setup(repo => repo.UpdateRoleAsync(userId, role)).ReturnsAsync(true);

            // Act
            bool result = await _userService.UpdateRoleAsync(userId, role);

            // Assert
            Assert.That(result, Is.True, "La actualización del rol debería retornar true para roles válidos.");
            _userRepositoryMock.Verify(repo => repo.UpdateRoleAsync(userId, role), Times.Once);
        }

        [Test]
        public async Task UpdateRoleAsync_ShouldReturnFalse_WhenRoleIsInvalid()
        {
            // Arrange
            string userId = "user123";
            string role = "invalid_role";

            // Act
            bool result = await _userService.UpdateRoleAsync(userId, role);

            // Assert
            Assert.That(result, Is.False, "La actualización del rol debería retornar false para roles inválidos.");
            _userRepositoryMock.Verify(repo => repo.UpdateRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task UpdateRoleAsync_ShouldReturnFalse_WhenRoleIsNullOrEmpty()
        {
            // Arrange
            string userId = "user123";

            // Act
            bool resultNull = await _userService.UpdateRoleAsync(userId, null);
            bool resultEmpty = await _userService.UpdateRoleAsync(userId, "");

            // Assert
            Assert.That(resultNull, Is.False, "La actualización del rol debería retornar false cuando el rol es null.");
            Assert.That(resultEmpty, Is.False, "La actualización del rol debería retornar false cuando el rol es una cadena vacía.");
            _userRepositoryMock.Verify(repo => repo.UpdateRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Tests for ValidateUserData

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            // Arrange
            User user = new User
            {
                Name = "",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "Password1!"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("El nombre es obligatorio."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenLastNameIsEmpty()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "Password1!"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("El apellido es requerido es obligatorio."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenUsernameIsEmpty()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "",
                Email = "john@example.com",
                Password = "Password1!"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("El nombre de usuario es obligatorio."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenEmailIsEmpty()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "",
                Password = "Password1!"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("El correo electrónico es obligatorio."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenPasswordIsEmpty()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = ""
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("La contraseña es obligatoria."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenEmailIsInvalid()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "invalid-email",
                Password = "Password1!"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("El formato del correo electrónico es inválido."));
        }

        [Test]
        public void CreateUserAsync_ShouldThrowArgumentException_WhenPasswordIsInvalid()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "password" // Inválida: sin mayúsculas, números ni caracteres especiales
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("La constraseña debe incluir al menos una mayúscula, una minúscula, un número y un carácter especial."));
        }

        [Test]
        public async Task CreateUserAsync_ShouldThrowArgumentException_WhenEmailAlreadyExists()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "Password1!"
            };

            _userRepositoryMock.Setup(repo => repo.CheckExistingUserAsync(user.Email, user.Username))
                .ReturnsAsync("Email");

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("Correo electrónico ya está en uso."));
        }

        [Test]
        public async Task CreateUserAsync_ShouldThrowArgumentException_WhenUsernameAlreadyExists()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "Password1!"
            };

            _userRepositoryMock.Setup(repo => repo.CheckExistingUserAsync(user.Email, user.Username))
                .ReturnsAsync("Username");

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("Nombre de usuario ya está en uso."));
        }

        [Test]
        public async Task CreateUserAsync_ShouldCreateUser_WhenDataIsValid()
        {
            // Arrange
            User user = new User
            {
                Name = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "Password1!"
            };

            _userRepositoryMock.Setup(repo => repo.CheckExistingUserAsync(user.Email, user.Username))
                .ReturnsAsync((string)null); // No existe usuario con ese email o username
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            bool result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.That(result, Is.True, "La creación del usuario debería retornar true cuando los datos son válidos.");
            _userRepositoryMock.Verify(repo => repo.CheckExistingUserAsync(user.Email, user.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.Is<User>(u =>
                u.Name == user.Name &&
                u.LastName == user.LastName &&
                u.Username == user.Username &&
                u.Email == user.Email &&
                u.Role == "cli" && // Se establece el rol por defecto en "cli"
                u.IsVerified == false
            )), Times.Once);
        }

        #endregion
    }
}

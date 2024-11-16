using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Text.RegularExpressions;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;


        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            ValidateUserData(user);
            string existingUser = await _userRepository.CheckExistingUserAsync(user.Email, user.Username);
            if (existingUser == "Email")
                throw new ArgumentException("Correo electrónico ya está en uso.");
            else if (existingUser == "Username")
                throw new ArgumentException("Nombre de usuario ya está en uso.");
            user.IsVerified = false;
            user.Role = "cli";
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<(User, string token)> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);
            if (user != null)
            {
                var token = _tokenService.GenerateToken(user);
                return (user, token);
            }
            return (null, null);
        }

        public async Task<List<UserDevDto>> GetAllUserDevAsync()
        {
            List<UserDevDto> userDevDtos = new List<UserDevDto>();
            List<User> users = await _userRepository.GetUsersAsync();
            foreach (User user in users)
            {
                UserDevDto userDevDto = new()
                {
                    User = user,
                    AssociatedCompanies = await GetUserEnterprises(user)
                };
                userDevDtos.Add(userDevDto);
            }
            return userDevDtos;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            ValidateUpdateUserRequest(request);
            UpdateUserRequest trimmedRequest = await TrimUnchangedAttributes(request);
            if (request.Password != null)
            {
                if (IsValidPassword(trimmedRequest.Password))
                {
                    throw new ArgumentException("La constraseña debe incluir al menos una mayúscula, una minúscula, un número y un carácter especial.");
                }
            }
            if (trimmedRequest != null) {
                return await _userRepository.UpdateUserAsync(trimmedRequest);
            }
            return false;
        }

        public async Task<bool> UpdateRoleAsync(string id, string role)
        {
            var valid = ValidateUserRole(role);
            if (valid)
            {
                return await _userRepository.UpdateRoleAsync(id, role);
            }
            return false;
        }

        public async Task<bool> UpdateAccountVerification(string Id)
        {
            return await _userRepository.UpdateAccountVerification(Id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            User user = await _userRepository.GetUserByUsernameAsync(username);
            if (user != null) {
                return user;
            }
            return null;
        }


        // Métodos auxiliares, pueden estar en otra clase


        private async Task<List<Enterprise>> GetUserEnterprises(User user)
        {
            return await _userRepository.GetUserEnterpisesAsync(user);
        }

        private bool ValidateUserRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return false;
            if(role == "cli" || role == "emp" || role == "dev")
            {
                return true;
            }
            return false;
        }

        private void ValidateUserData(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("El apellido es requerido es obligatorio.");
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("El correo electrónico es obligatorio.");
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("La contraseña es obligatoria.");
            if (!IsValidEmail(user.Email))
                throw new ArgumentException("El formato del correo electrónico es inválido.");
            if (!IsValidPassword(user.Password))
                throw new ArgumentException("La constraseña debe incluir al menos una mayúscula, una minúscula, un número y un carácter especial.");
        }

        private void ValidateUpdateUserRequest(UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(request.Username))
                throw new ArgumentException("Nombre de usuario no válido para actualizar.");
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Nombre no válido para actualizar.");
            if (string.IsNullOrEmpty(request.LastName))
                throw new ArgumentException("Apellido no válido para actualizar.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            try
            {
                string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{1,16}$";
                Regex regex = new Regex(pattern);
                return regex.IsMatch(password);
            }
            catch
            {
                return false;
            }
        }

        private async Task<UpdateUserRequest> TrimUnchangedAttributes(UpdateUserRequest request)
        {
            UpdateUserRequest trimmedRequest = new UpdateUserRequest();
            User user = await _userRepository.GetUserByIdAsync(request.Id);
            trimmedRequest.Id = request.Id;
            trimmedRequest.Username = request.Username != user.Username ? request.Username : null;
            trimmedRequest.Name = request.Name != user.Name ? request.Name : null;
            trimmedRequest.LastName = request.LastName != user.LastName ? request.LastName : null;
            trimmedRequest.Password = request.Password != user.Password ? request.Password : null;
            if (string.IsNullOrEmpty(trimmedRequest.Password))
                trimmedRequest.Password = null;
            return trimmedRequest;
        }
    }
}

using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(string Id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
        Task<List<Enterprise>> GetUserEnterpisesAsync(User user);
        Task<string> CheckExistingUserAsync(string email, string username);
        Task<bool> UpdateUserAsync(UpdateUserRequest request);
        Task<bool> UpdateAccountVerification(string id);
        Task<bool> UpdateRoleAsync(string id, string role);
    }
}

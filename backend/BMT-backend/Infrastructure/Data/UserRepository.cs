using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            var query = "INSERT INTO dbo.Users (Name, LastName, Username, Email, IsVerified, Password) VALUES (@Name, @LastName, @Username, @Email, @IsVerified, @Password)";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@IsVerified", user.IsVerified);
                command.Parameters.AddWithValue("@Password", user.Password);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            var query = "SELECT * FROM dbo.Users";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        IsVerified = (bool)reader["IsVerified"],
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString(),
                        ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                    });
                }
            }
            return users;
        }

        public async Task<User> GetUserByIdAsync(string Id)
        {
            User user = null;
            var query = "SELECT * FROM dbo.Users WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                command.Parameters.AddWithValue("@Id", Id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Username = reader["UserName"].ToString(),
                            Email = reader["Email"].ToString(),
                            IsVerified = (bool)reader["IsVerified"],
                            Password = reader["Password"].ToString(),
                            Role = reader["Role"].ToString(),
                            ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                        };
                    }
                }
            }
            return user;
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            User user = null;
            var query = "SELECT * FROM dbo.Users WHERE Email = @Email AND Password = @Password";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            IsVerified = (bool)reader["IsVerified"],
                            Password = reader["Password"].ToString(),
                            Role = reader["Role"].ToString(),
                            ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                        };
                    }
                }
            }
            return user;
        }

        public async Task<List<Enterprise>> GetUserEnterpisesAsync(User user)
        {
            List<Enterprise> enterprises = new List<Enterprise>();
            var query = "EXEC GetEnterprisesByUser @UserId";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
            query, connection))
            {
                command.Parameters.AddWithValue("@UserId", user.Id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        enterprises.Add(new Enterprise
                        {
                            Id = reader["Id"].ToString(),
                            IdentificationType = (int)reader["IdentificationType"],
                            IdentificationNumber = reader["IdentificationNumber"].ToString(),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString()
                        });
                    }
                }
                return enterprises;
            }
        }

        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            var query = "EXEC UpdateUser @Id, @Username, @Name, @LastName, @Password";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", request.Id);
                command.Parameters.AddWithValue("@Username", (object)request.Username ?? DBNull.Value);
                command.Parameters.AddWithValue("@Name", (object)request.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", (object)request.LastName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Password", (object)request.Password ?? DBNull.Value);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }
        }
 
        public async Task<bool> UpdateAccountVerification(string id)
        {
            var query = "UPDATE dbo.Users SET IsVerified = 1 WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }
        }

        public async Task<bool> UpdateRoleAsync(string id, string role)
        {
            var query = "UPDATE dbo.Users SET Role = @Role WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Role", role);
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                bool result = command.ExecuteNonQuery() >= 1;
                return result;
            }
        }

        public async Task<string> CheckExistingUserAsync(string email, string username)
        {
            var query = "EXEC CheckUserExists @Email = @Email, @UserName = @UserName";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@UserName", username);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var result = reader["Result"].ToString();
                        return result;
                    }
                }
            }
            return null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            User user = null;
            var query = "SELECT * FROM dbo.Users WHERE UserName = @username";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            IsVerified = (bool)reader["IsVerified"],
                            Password = reader["Password"].ToString(),
                            Role = reader["Role"].ToString(),
                            ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                        };
                    }
                }
            }
            return user;
        }
    }
}

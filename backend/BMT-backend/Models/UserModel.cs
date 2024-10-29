using System.Text.Json.Serialization;

namespace BMT_backend.Models
{
    public class UserModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
        public string? ProfilePictureURL { get; set; }
    }
    public class UpdateUserProfileModel
    {
        public string Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
    }
}

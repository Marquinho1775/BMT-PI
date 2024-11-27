using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.DTOs
{
    public class UserDevDto
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<string> AssociatedCompanies { get; set; }
    }
}

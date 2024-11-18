using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.DTOs
{
    public class UserDevDto
    {
        public User User { get; set; }
        public List<Enterprise> AssociatedCompanies { get; set; }
    }
}

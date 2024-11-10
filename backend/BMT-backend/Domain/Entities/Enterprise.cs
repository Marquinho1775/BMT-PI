using System.Text.Json.Serialization;
namespace BMT_backend.Domain.Entities
{
    public class Enterprise
    {
        public string Id { get; set; } = null!;
        public int IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Entrepreneur? Administrator { get; set; }
        public List<Entrepreneur>? Staff { get; set; }

    }
}

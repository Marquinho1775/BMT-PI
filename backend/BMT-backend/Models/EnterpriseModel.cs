using System.Text.Json.Serialization;
namespace BMT_backend.Models
{
    public class EnterpriseModel
    {
        public string Id { get; set; } = null!;
        public int IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public EntrepreneurViewModel? Administrator { get; set; }
        public List<EntrepreneurViewModel>? Staff { get; set; }

    }

    public class EnterpriseViewModel
    {
        public string Id { get; set; }
        public string EnterpriseName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public string AdminLastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateEnterpriseModel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

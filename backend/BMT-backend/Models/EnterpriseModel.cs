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
        public EntrepreneurViewModel? Administrator { get; set; }
        public List<EntrepreneurViewModel>? Staff { get; set; }

        // public List<ProductModel> Catalog { get; set; }

    }

    public class EnterpriseViewModel
    {
        public string EnterpriseName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; }
        public string AdminLastName { get; set; }
    }
}

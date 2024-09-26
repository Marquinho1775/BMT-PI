namespace BMT_backend.Models
{
    public class EnterpriseModel
    {
        public string Id { get; set; } = null!;
        public int IdentificationType { get; set; }
        public string IdentificationNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EntrepreneurModel Admininstrator { get; set; }
        public List<EntrepreneurModel> Staff { get; set; }

        // public List<ProductModel> Catalog { get; set; }

    }
}

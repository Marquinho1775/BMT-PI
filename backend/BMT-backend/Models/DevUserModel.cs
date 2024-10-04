namespace BMT_backend.Models
{
    public class DevUserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Identification { get; set; }
        public string[] AssociatedCompanies { get; set; }
    }
}

namespace BMT_backend.Models
{
    public class EntrepreneurModel : UserModel
    {
        public string Id { get; set; } = null!;
        public string Identification { get; set; }
        // public List<EnterpriseModel>? EnterprisesWorkingIn { get; set; }
    }
}

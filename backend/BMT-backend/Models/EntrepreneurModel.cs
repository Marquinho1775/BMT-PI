namespace BMT_backend.Models
{
    public class EntrepreneurModel : UserModel
    {   
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Identification { get; set; }
        // public List<EnterpriseModel>? EnterprisesWorkingIn { get; set; }
    }
}

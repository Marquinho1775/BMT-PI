namespace BMT_backend.Models
{
    public class EntrepreneurModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; }
        public string Identification { get; set; }
    }
    public class EntrepreneurViewModel : EntrepreneurModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
    }
    public class AddEntrepreneurToEnterpriseRequest
    {
        public string EntrepreneurIdentification { get; set; }
        public string EnterpriseIdentification { get; set; }
        public bool IsAdmin { get; set; }
    }
}

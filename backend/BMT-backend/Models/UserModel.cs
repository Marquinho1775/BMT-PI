namespace BMT_backend.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string Password { get; set; }
        public bool IsEntrepeneur { get; set; }
        public string Identification { get; set; }
    }
}

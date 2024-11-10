namespace BMT_backend.Domain.Entities
{
    public class Entrepreneur
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; }
        public string Identification { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool? IsVerified { get; set; }
    }
}

namespace BMT_backend.Domain.Requests
{
    public class UpdateEnterpriseRequest
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

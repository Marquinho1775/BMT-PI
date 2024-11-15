namespace BMT_backend.Presentation.Requests
{
    public class UpdateUserRequestDto
    {
        public string Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
    }
}

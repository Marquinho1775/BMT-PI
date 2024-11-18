namespace BMT_backend.Domain.Entities
{
    public class ImageFile
    {
        public string OwnerId { get; set; }
        public string OwnerType { get; set; }
        public IFormFile Image { get; set; }
    }
}

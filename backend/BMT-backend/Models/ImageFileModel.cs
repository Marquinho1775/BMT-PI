namespace BMT_backend.Models
{
    public class ImageFileModel
    {
        public string OwnerId { get; set; }
        public string OwnerType { get; set; }
        public IFormFile Image { get; set; }
    }
}

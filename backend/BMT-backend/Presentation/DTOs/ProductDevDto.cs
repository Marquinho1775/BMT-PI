namespace BMT_backend.Presentation.DTOs
{
    public class ProductDevDto
    {
        public string Name { get; set; }
        public string EnterpriseName { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string? Type { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? ImagesURLs { get; set; }
        public int? Stock { get; set; }
        public int? Limit { get; set; }
        public string? WeekDaysAvailable { get; set; }
    }
}

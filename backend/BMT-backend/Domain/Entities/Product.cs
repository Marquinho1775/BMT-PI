namespace BMT_backend.Domain.Entities
{
    public class Product
    {
        public string? Id { get; set; } = null!;
        public string EnterpriseId { get; set; }
        public string? EnterpriseName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string? Type { get; set; }
        public List<string>? Tags { get; set; }
        public List<IFormFile>? ImagesFiles { get; set; }
        public List<string>? ImagesURLs { get; set; }
        public int? Stock { get; set; }
        public int? Limit { get; set; }
        public string? WeekDaysAvailable { get; set; }
    }
}
namespace BMT_backend.Models
{
    public class ProductModel
    {
        public string Id { get; set; } = null!;
        public string EnterpriseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; } 
        public double Price { get; set; }
        public string Type { get; set; } 
        public List<string> Tags { get; set; }
        public List<string> ImagesURLs { get; set; }
        public int? Stock { get; set; }
        public int? Limit { get; set; }
        public string? WeekDaysAvailable { get; set; } 
    }

    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double Price { get; set; }
        public string EnterpriseName { get; set; }
        public List<string> Tags { get; set; }
        public List<string> ImagesURLs { get; set; }
    }

    public class CartProductModel
    {
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
    }
}


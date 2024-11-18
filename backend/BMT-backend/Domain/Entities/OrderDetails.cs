using BMT_backend.Domain.Entities;

namespace BMT_backend.Domain.Entities
{
    public class OrderDetails
    {
        public Order Order { get; set; }
        public string? UserName { get; set; } = null!;
        public string? Direction { get; set; } = null!;
        public string? UserEmail { get; set; } = null!;
        public string? OtherSigns { get; set; }
        public string? Coordinates { get; set; } = null!;
        public List<ProductDetails> Products { get; set; } = new List<ProductDetails>();
    }
}

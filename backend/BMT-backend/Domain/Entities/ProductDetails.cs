namespace BMT_backend.Domain.Entities
{
    public class ProductDetails
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public double ProductsCost { get; set; }
        public string EnterpriseName { get; set; } = null!;
        public string EnterpriseEmail { get; set; } = null!;
    }
}

namespace BMT_backend.Domain.Entities
{
    public class ProductDetails
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public int ProductsCost { get; set; }
        public string EnterpriseName { get; set; } = null!;
        public string EnterpriseEmail { get; set; } = null!;
    }
}

namespace BMT_backend.Domain.Requests
{
    public class ProductStockRequest
    {
        public string ProductId { get; set; }
        public string Type { get; set; }
        public string? Date { get; set; }
    }
}

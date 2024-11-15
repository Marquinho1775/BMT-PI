namespace BMT_backend.Presentation.Requests
{
    public class UpdateProductStockRequest
    {
        public string ProductId { get; set; }
        public string? Date { get; set; }
        public int Quantity { get; set; }
    }
}

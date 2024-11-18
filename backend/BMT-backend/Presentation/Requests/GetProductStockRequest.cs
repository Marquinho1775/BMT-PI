namespace BMT_backend.Presentation.Requests
{
    public class GetProductStockRequest
    {
        public string ProductId { get; set; }
        public string Type { get; set; }
        public string? Date { get; set; }
    }
}

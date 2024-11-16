public class UpdateProductStockRequest
{
    public string ProductId { get; set; }
    public string? DateString { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; }
}

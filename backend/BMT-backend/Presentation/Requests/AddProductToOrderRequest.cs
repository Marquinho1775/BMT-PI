namespace BMT_backend.Presentation.Requests
{
    public class AddProductToOrderRequest
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Amount { get; set; }
        public double ProductsCost { get; set; }
    }
}

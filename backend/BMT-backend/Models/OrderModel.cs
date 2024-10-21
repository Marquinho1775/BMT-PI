namespace BMT_backend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryAddress { get; set; } // Consider adding separate fields for street, city, state, etc. if needed

        // Navigation property (optional)
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
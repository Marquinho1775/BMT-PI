namespace BMT_backend.Domain.Entities
{
    public class CartProduct
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }
    }
}

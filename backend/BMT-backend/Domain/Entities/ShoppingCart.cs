
namespace BMT_backend.Domain.Entities
{
    public class ShoppingCart
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; }
        public List<CartProduct> CartProducts { get; set; }
        public double CartTotal { get; set; }
    }
}

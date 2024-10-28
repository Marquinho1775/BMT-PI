namespace BMT_backend.Models
{
    public class ShoppingCartModel
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; }
        public List<CartProductModel> CartProducts { get; set; }
        public double CartTotal { get; set; }
    }
}

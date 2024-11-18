using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<bool> CreateShoppingCartAsync(string userName);

        Task<ShoppingCart> GetShoppingCartAsync(string userId);

        Task<List<CartProduct>> GetCartProductsAsync(string shoppingCartId);

        Task<CartProduct> GetCartProductAsync(string shoppingCartId, string productId);

        Task<string> GetCartIdAsync(string userId);

        Task<string> AddProductToCartAsync(string shoppingCartId, string productId);

        Task<bool> ChangeProductQuantityAsync(string shoppingCartId, string productId, int quantity);

        Task<bool> DeleteProductFromCartAsync(string shoppingCartId, string productId);

        Task<bool> ClearShoppingCartAsync(string shoppingCartId);
    }
}

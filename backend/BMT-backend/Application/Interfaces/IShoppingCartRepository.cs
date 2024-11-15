using BMT_backend.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BMT_backend.Application.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<bool> CreateShoppingCartAsync(string userName);
        Task<DataTable> GetShoppingCartByUserIdAsync(string userId);
        Task<string> GetCartIdAsync(string userId);
        Task<DataTable> GetCartProductsAsync(string shoppingCartId);
        Task<string> GetCartIdByUserIdAsync(string userId);
        Task<string> AddProductToCartAsync(string shoppingCartId, string productId);
        Task<bool> UpdateProductQuantityAsync(string shoppingCartId, string productId, int quantity, double subtotal, double difference);
        Task<bool> RemoveProductFromCartAsync(string shoppingCartId, string productId, double substractTotal);
        Task<bool> ClearShoppingCartAsync(string shoppingCartId);
    }
}

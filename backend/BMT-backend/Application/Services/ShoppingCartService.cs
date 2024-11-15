using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data;
using System.Threading.Tasks;

namespace BMT_backend.Application.Services
{
    public class ShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
        }

        public async Task<DataTable> GetShoppingCartByUserIdAsync(string userId)
        {
            // Verifica si el carrito existe antes de devolverlo
            var cart = await _shoppingCartRepository.GetShoppingCartByUserIdAsync(userId);
            return cart?.Rows.Count > 0 ? cart : null;
        }

        public async Task<DataTable> GetCartProductsAsync(string shoppingCartId)
        {
            // Verifica si el carrito tiene productos antes de devolverlos
            var products = await _shoppingCartRepository.GetCartProductsAsync(shoppingCartId);
            return products?.Rows.Count > 0 ? products : null;
        }

        public async Task<string> AddProductToCartAsync(string shoppingCartId, string productId)
        {
            // Verifica que el producto exista antes de agregarlo al carrito
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return "ProductNotFound";
            }

            // Llama al repositorio del carrito para agregar el producto
            return await _shoppingCartRepository.AddProductToCartAsync(shoppingCartId, productId);
        }

        public async Task<bool> ChangeProductQuantityAsync(string shoppingCartId, string productId, int newQuantity)
        {
            // Obtiene el producto y verifica si existe
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Verifica que el stock sea suficiente
            var currentStock = await _productRepository.GetProductStock(new GetProductStockRequest { ProductId = productId });
            if (newQuantity > currentStock)
            {
                return false; // Stock insuficiente
            }

            // Obtiene la cantidad actual en el carrito
            var cartProducts = await _shoppingCartRepository.GetCartProductsAsync(shoppingCartId);
            var currentRow = cartProducts.Select($"ProductId = '{productId}'");
            if (currentRow.Length == 0)
            {
                return false; // El producto no está en el carrito
            }

            int currentQuantity = (int)currentRow[0]["Quantity"];
            double currentSubtotal = (double)currentRow[0]["Subtotal"];

            // Calcula el subtotal nuevo y la diferencia para actualizar el total del carrito
            double newSubtotal = product.Price * newQuantity;
            double difference = newSubtotal - currentSubtotal;

            // Actualiza la cantidad y el subtotal en el carrito
            return await _shoppingCartRepository.UpdateProductQuantityAsync(shoppingCartId, productId, newQuantity, newSubtotal, difference);
        }

        public async Task<bool> RemoveProductFromCartAsync(string shoppingCartId, string productId)
        {
            // Obtiene el producto y verifica que esté en el carrito
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Calcula el total que se va a restar y elimina el producto del carrito
            double substractTotal = product.Price * -1;
            return await _shoppingCartRepository.RemoveProductFromCartAsync(shoppingCartId, productId, substractTotal);
        }

        public async Task<bool> ClearShoppingCartAsync(string shoppingCartId)
        {
            return await _shoppingCartRepository.ClearShoppingCartAsync(shoppingCartId);
        }

        // Método para obtener el ID del carrito por UserId
        public async Task<string> GetCartIdAsync(string userId)
        {
            return await _shoppingCartRepository.GetCartIdAsync(userId);
        }

        // Método para crear un carrito de compras para un usuario
        public async Task<bool> CreateShoppingCartAsync(string userId)
        {
            return await _shoppingCartRepository.CreateShoppingCartAsync(userId);
        }
    }
}

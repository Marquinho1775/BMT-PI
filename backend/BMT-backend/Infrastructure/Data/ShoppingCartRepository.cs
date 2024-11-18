using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {

        private readonly string _connectionString;
        private readonly IProductRepository _productRepository;

        public ShoppingCartRepository(string connectionString)
        {
            _connectionString = connectionString;
            _productRepository = new ProductRepository(connectionString);
        }

        public async Task<bool> CreateShoppingCartAsync(string userName)
        {
            var query = "INSERT INTO ShoppingCarts (UserId, Total) " +
                        "SELECT Id, 0 FROM Users WHERE UserName = @userName";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userName", userName);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string userId)//queres quedarte quiero?
        {
            var query = "SELECT Id, UserId, Total " +
                        "FROM ShoppingCarts WHERE UserId = @userId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }
            await reader.ReadAsync();
            var shoppingCart = new ShoppingCart
            {
                Id = reader["Id"].ToString(),
                UserId = reader["UserId"].ToString(),
                CartProducts = await GetCartProductsAsync(reader["Id"].ToString()),
                CartTotal = double.Parse(reader["Total"].ToString())
            };
            return shoppingCart;
        }

        public async Task<List<CartProduct>> GetCartProductsAsync(string shoppingCartId)
        {
            List<CartProduct> cartProducts = new List<CartProduct>();
            var query = "SELECT ProductId, Quantity, Subtotal " +
                        "FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId";

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var cartProduct = new CartProduct
                {
                    Product = await _productRepository.GetProductDetailsByIdAsync(reader["ProductId"].ToString()),
                    Quantity = int.Parse(reader["Quantity"].ToString()),
                    Subtotal = double.Parse(reader["Subtotal"].ToString())
                };
                cartProducts.Add(cartProduct);
            }
            return cartProducts;
        }


        public async Task<CartProduct> GetCartProductAsync(string shoppingCartId, string productId)
        {
            var query = "SELECT ProductId, Quantity, Subtotal " +
                        "FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            command.Parameters.AddWithValue("@productId", productId);
            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }
            await reader.ReadAsync();
            var cartProduct = new CartProduct
            {
                Product = await _productRepository.GetProductByIdAsync(reader["ProductId"].ToString()),
                Quantity = int.Parse(reader["Quantity"].ToString()),
                Subtotal = double.Parse(reader["Subtotal"].ToString())
            };
            return cartProduct;
        }

        public async Task<string> GetCartIdAsync(string userId)
        {
            var query = "SELECT Id FROM ShoppingCarts WHERE UserId = @userId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            return command.ExecuteScalar().ToString();
        }

        public async Task<string> AddProductToCartAsync(string shoppingCartId, string productId)
        {
            try
            {
                var query = "IF EXISTS (" +
                            "SELECT 1 " +
                            "FROM ShoppingCartProducts " +
                            "WHERE ShoppingCartId = @shoppingCartId " +
                            "AND ProductId = @productId" +
                            ") " +
                            "SELECT 'ProductExists' " +
                            "ELSE " +
                            "BEGIN " +
                            "DECLARE @productPrice FLOAT = (SELECT Price FROM Products WHERE Id = @productId); " +
                            "DECLARE @subtotal FLOAT = @productPrice; " +
                            "INSERT INTO ShoppingCartProducts (ShoppingCartId, ProductId, Quantity, Subtotal) " +
                            "VALUES (@shoppingCartId, @productId, 1, @subtotal); " +
                            "UPDATE ShoppingCarts " +
                            "SET Total = Total + @subtotal " +
                            "WHERE Id = @shoppingCartId; " +
                            "SELECT 'Success' AS Result; " +
                            "END";
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                command.Parameters.AddWithValue("@productId", productId);
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> ChangeProductQuantityAsync(string shoppingCartId, string productId, int quantity)
        {
            CartProduct cartProduct = await GetCartProductAsync(shoppingCartId, productId);
            double subtotal = cartProduct.Product.Price * quantity;
            double difference = subtotal - cartProduct.Subtotal;

            var cartProductQuery = "UPDATE ShoppingCartProducts SET Quantity = @quantity, Subtotal = @subtotal " +
                                  "WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(cartProductQuery, connection);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@subtotal", subtotal);
            command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            command.Parameters.AddWithValue("@productId", productId);
            int rowsAffected = await command.ExecuteNonQueryAsync();

            var updateTotalQuery = "UPDATE ShoppingCarts SET Total = Total + @difference " +
                                "WHERE Id = @shoppingCartId";
            using var updateTotalCommand = new SqlCommand(updateTotalQuery, connection);
            updateTotalCommand.Parameters.AddWithValue("@difference", difference);
            updateTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            rowsAffected += await updateTotalCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteProductFromCartAsync(string shoppingCartId, string productId)
        {
            CartProduct cartProduct = await GetCartProductAsync(shoppingCartId, productId);
            double substractTotal = cartProduct.Subtotal;
            var cartProductQuery = "DELETE FROM ShoppingCartProducts " +
                                  "WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(cartProductQuery, connection);
            command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            command.Parameters.AddWithValue("@productId", productId);
            int rowsAffected = await command.ExecuteNonQueryAsync();

            var substractQuery = "UPDATE ShoppingCarts SET Total = Total - @substractTotal " +
                            "WHERE Id = @shoppingCartId";
            using var substractCommand = new SqlCommand(substractQuery, connection);
            substractCommand.Parameters.AddWithValue("@substractTotal", substractTotal);
            substractCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            rowsAffected += await substractCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ClearShoppingCartAsync(string shoppingCartId)
        {
            var query = "DELETE FROM ShoppingCartProducts " +
                        "WHERE ShoppingCartId = @shoppingCartId";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            int rowsAffected = await command.ExecuteNonQueryAsync();

            var updateTotalQuery = "UPDATE ShoppingCarts SET Total = 0 " +
                                "WHERE Id = @shoppingCartId";
            using var updateTotalCommand = new SqlCommand(updateTotalQuery, connection);
            updateTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
            rowsAffected += await updateTotalCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

}
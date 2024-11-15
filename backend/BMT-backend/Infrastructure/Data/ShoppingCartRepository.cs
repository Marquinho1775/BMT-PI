using BMT_backend.Domain.Entities;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BMT_backend.Application.Interfaces;

namespace BMT_backend.Infrastructure.Data
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly string _connectionString;

        public ShoppingCartRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateShoppingCartAsync(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO ShoppingCarts (UserId, Total) SELECT Id, 0 FROM Users WHERE UserName = @userName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userName", userName);
                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<DataTable> GetShoppingCartByUserIdAsync(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, UserId, Total FROM ShoppingCarts WHERE UserId = @userId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        public async Task<DataTable> GetCartProductsAsync(string shoppingCartId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT ProductId, Quantity, Subtotal FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        public async Task<string> GetCartIdByUserIdAsync(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id FROM ShoppingCarts WHERE UserId = @userId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    return (await command.ExecuteScalarAsync())?.ToString();
                }
            }
        }

        public async Task<string> AddProductToCartAsync(string shoppingCartId, string productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    IF EXISTS (
                        SELECT 1 
                        FROM ShoppingCartProducts 
                        WHERE ShoppingCartId = @shoppingCartId 
                        AND ProductId = @productId
                    )
                    SELECT 'ProductExists'
                    ELSE
                    BEGIN
                        DECLARE @productPrice FLOAT = (SELECT Price FROM Products WHERE Id = @productId);
                        DECLARE @subtotal FLOAT = @productPrice;

                        INSERT INTO ShoppingCartProducts (ShoppingCartId, ProductId, Quantity, Subtotal)
                        VALUES (@shoppingCartId, @productId, 1, @subtotal);

                        UPDATE ShoppingCarts 
                        SET Total = Total + @subtotal 
                        WHERE Id = @shoppingCartId;

                        SELECT 'Success' AS Result;
                    END
                ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    command.Parameters.AddWithValue("@productId", productId);
                    return (await command.ExecuteScalarAsync())?.ToString();
                }
            }
        }

        public async Task<bool> UpdateProductQuantityAsync(string shoppingCartId, string productId, int quantity, double subtotal, double difference)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string updateProductQuery = "UPDATE ShoppingCartProducts SET Quantity = @quantity, Subtotal = @subtotal WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
                using (var updateProductCommand = new SqlCommand(updateProductQuery, connection))
                {
                    updateProductCommand.Parameters.AddWithValue("@quantity", quantity);
                    updateProductCommand.Parameters.AddWithValue("@subtotal", subtotal);
                    updateProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    updateProductCommand.Parameters.AddWithValue("@productId", productId);
                    int rowsAffected = await updateProductCommand.ExecuteNonQueryAsync();

                    string updateCartTotalQuery = "UPDATE ShoppingCarts SET Total = Total + @difference WHERE Id = @shoppingCartId";
                    using (var updateCartCommand = new SqlCommand(updateCartTotalQuery, connection))
                    {
                        updateCartCommand.Parameters.AddWithValue("@difference", difference);
                        updateCartCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                        rowsAffected += await updateCartCommand.ExecuteNonQueryAsync();
                    }

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> RemoveProductFromCartAsync(string shoppingCartId, string productId, double substractTotal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string deleteProductQuery = "DELETE FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId AND ProductId = @productId";
                using (var deleteProductCommand = new SqlCommand(deleteProductQuery, connection))
                {
                    deleteProductCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    deleteProductCommand.Parameters.AddWithValue("@productId", productId);
                    int rowsAffected = await deleteProductCommand.ExecuteNonQueryAsync();

                    string updateCartTotalQuery = "UPDATE ShoppingCarts SET Total = Total - @substractTotal WHERE Id = @shoppingCartId";
                    using (var updateCartCommand = new SqlCommand(updateCartTotalQuery, connection))
                    {
                        updateCartCommand.Parameters.AddWithValue("@substractTotal", substractTotal);
                        updateCartCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                        rowsAffected += await updateCartCommand.ExecuteNonQueryAsync();
                    }

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> ClearShoppingCartAsync(string shoppingCartId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string deleteProductsQuery = "DELETE FROM ShoppingCartProducts WHERE ShoppingCartId = @shoppingCartId";
                using (var deleteProductsCommand = new SqlCommand(deleteProductsQuery, connection))
                {
                    deleteProductsCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                    int rowsAffected = await deleteProductsCommand.ExecuteNonQueryAsync();

                    string resetTotalQuery = "UPDATE ShoppingCarts SET Total = 0 WHERE Id = @shoppingCartId";
                    using (var resetTotalCommand = new SqlCommand(resetTotalQuery, connection))
                    {
                        resetTotalCommand.Parameters.AddWithValue("@shoppingCartId", shoppingCartId);
                        rowsAffected += await resetTotalCommand.ExecuteNonQueryAsync();
                    }

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<string> GetCartIdAsync(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT ShoppingCartId FROM ShoppingCarts WHERE UserId = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
        }
    }
}

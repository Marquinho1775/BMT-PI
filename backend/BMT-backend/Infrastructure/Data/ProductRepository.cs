using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data.SqlClient;
using System.Text;

namespace BMT_backend.Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ProductRepository()
        {
        }

        public async Task<bool> CreateBaseProductAsync(Product product)
        {
            var query = @"
        INSERT INTO Products (EnterpriseId, Name, Description, Price, Weight)
        OUTPUT Inserted.Id
        VALUES (@EnterpriseId, @Name, @Description, @Price, @Weight);";
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@EnterpriseId", product.EnterpriseId);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Weight", product.Weight);
            await connection.OpenAsync();
            var id = await command.ExecuteScalarAsync();
            product.Id = id.ToString();
            return true;
        }


        public async Task<bool> CreateNonPerishableProductAsync(string productId, int Stock)
        {
            var query = "INSERT INTO NonPerishableProducts (ProductId, Stock) VALUES (@ProductId, @Stock);";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@Stock", Stock);
            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected >= 1;
        }

        public async Task<bool> CreatePerishableProductAsync(string productId, int limit, string WeekDaysAvailable)
        {
            var query = "INSERT INTO PerishableProducts(ProductId, Limit, WeekDaysAvailable) VALUES(@ProductId, @Limit, @WeekDaysAvailable); ";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@Limit", limit);
            command.Parameters.AddWithValue("@WeekDaysAvailable", WeekDaysAvailable);
            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected >= 1;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var query = "SELECT Id, EnterpriseId, Name, Description, Price, Weight FROM Products AND SoftDeleted = 0;";
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = reader["Id"].ToString(),
                        EnterpriseId = reader["EnterpriseId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDouble(reader["Price"]),
                        Weight = Convert.ToDouble(reader["Weight"]),
                    });
                }
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var query = "SELECT Id, EnterpriseId, Name, Description, Price, Weight FROM Products where Id = @Id AND SoftDeleted = 0;";
            var product = new Product();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    product.Id = reader["Id"].ToString();
                    product.EnterpriseId = reader["EnterpriseId"].ToString();
                    product.Name = reader["Name"].ToString();
                    product.Description = reader["Description"].ToString();
                    product.Price = Convert.ToDouble(reader["Price"]);
                    product.Weight = Convert.ToDouble(reader["Weight"]);
                }
            }
            return product;
        }


        public async Task<List<Product>> GetProductsDetailsAsync()
        {
            var query = "SELECT Id FROM Products WHERE SoftDeleted = 0;";
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var product = await GetProductDetailsByIdAsync(reader["Id"].ToString());
                    if (product != null)
                        products.Add(product);
                }
            }
            return products;
        }

        public async Task<Product> GetProductDetailsByIdAsync(string productId)
        {
            const string storedProcedure = "EXEC GetProductDetails @ProductId";
            var parameters = new SqlParameter("@ProductId", productId);
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);
            command.Parameters.Add(parameters);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            Product product = null;
            if (await reader.ReadAsync())
            {
                product = new Product
                {
                    Id = reader["Id"].ToString(),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Weight = Convert.ToDouble(reader["Weight"]),
                    Price = Convert.ToDouble(reader["Price"]),
                    EnterpriseId = reader["EnterpriseId"].ToString(),
                    EnterpriseName = reader["EnterpriseName"].ToString(),
                    Type = reader["ProductType"].ToString(),
                    Stock = reader["Stock"] != DBNull.Value ? Convert.ToInt32(reader["Stock"]) : (int?)null,
                    Limit = reader["Limit"] != DBNull.Value ? Convert.ToInt32(reader["Limit"]) : (int?)null,
                    WeekDaysAvailable = reader["WeekDaysAvailable"] != DBNull.Value ? reader["WeekDaysAvailable"].ToString() : null
                };
            }
            else
            {
                return null;
            }
            if (await reader.NextResultAsync())
            {
                var tags = new List<string>();
                while (await reader.ReadAsync())
                {
                    tags.Add(reader["TagName"].ToString());
                }
                product.Tags = tags;
            }
            if (await reader.NextResultAsync())
            {
                var images = new List<string>();
                while (await reader.ReadAsync())
                {
                    images.Add(reader["ImageUrl"].ToString());
                }
                product.ImagesURLs = images;
            }
            return product;
        }

        public async Task<int> GetNonPerishableStock(string productId)
        {
            const string query = "SELECT Stock FROM NonPerishableProducts WHERE ProductId = @ProductId AND SoftDeleted = 0;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Convert.ToInt32(reader["Stock"]);
            }
            return 0;
        }

        public async Task<int> GetPerishableStock(string productId, string date)
        {
            const string query = @"SELECT ISNULL(dd.Stock, pp.Limit) AS Stock
                FROM PerishableProducts pp
                LEFT JOIN DateDisponibility dd ON dd.ProductId = pp.ProductId AND dd.Date = @Date
                WHERE pp.ProductId = @ProductId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Date", date);
            command.Parameters.AddWithValue("@ProductId", productId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Convert.ToInt32(reader["Stock"]);
            }
            return 0;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var updateQuery = "UPDATE Products SET " +
                "Name = @Name, Description = @Description, Weight = @Weight, Price = @Price " +
                "WHERE Id = @Id;";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", product.Id),
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Description", product.Description),
                new SqlParameter("@Weight", product.Weight),
                new SqlParameter("@Price", product.Price)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                updateQuery, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateNonPerishableDetailsAsync(string id, int quantity)
        {
            var query = @"
                UPDATE NonPerishableProducts
                SET Stock = @Stock
                WHERE ProductId = @ProductId;";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Stock", quantity),
                new SqlParameter("@ProductId", id)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePerishableDetailsAsync(string id, string weekDaysAvailable, int? Limit)
        {
            var queryBuilder = new StringBuilder("UPDATE PerishableProducts SET ");
            var parameters = new List<SqlParameter>();
            if (Limit.HasValue)
            {
                queryBuilder.Append("Limit = @Limit, ");
                parameters.Add(new SqlParameter("@Limit", Limit.Value));
            }
            if (!string.IsNullOrEmpty(weekDaysAvailable))
            {
                queryBuilder.Append("WeekDaysAvailable = @WeekDaysAvailable, ");
                parameters.Add(new SqlParameter("@WeekDaysAvailable", weekDaysAvailable));
            }
            if (parameters.Count > 0)
            {
                queryBuilder.Length -= 2;
                queryBuilder.Append(" WHERE ProductId = @ProductId");
                parameters.Add(new SqlParameter("@ProductId", id));
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(queryBuilder.ToString(), connection);
                command.Parameters.AddRange(parameters.ToArray());
                await connection.OpenAsync();
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            return false;
        }

        public async Task<bool> UpdateNonPerishableStockAsync(string productId, int quantity)
        {
            const string query = @"
                UPDATE NonPerishableProducts
                SET Stock = Stock - @Quantity
                WHERE ProductId = @ProductId;";
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", productId),
                new("@Quantity", quantity)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;

        }
        public async Task<bool> UpdatePerishableStockAsync(string productId, string date, int quantity)
        {
            const string query = @"
                IF EXISTS (SELECT 1 FROM DateDisponibility WHERE ProductId = @ProductId AND Date = @Date)
                BEGIN
                    UPDATE DateDisponibility
                    SET Stock = Stock - @Quantity
                    WHERE ProductId = @ProductId AND Date = @Date;
                END
                ELSE
                BEGIN
                    INSERT INTO DateDisponibility (ProductId, Date, Stock)
                    VALUES (
                        @ProductId,
                        @Date,
                        (SELECT [Limit] FROM PerishableProducts WHERE ProductId = @ProductId) - @Quantity
                    );
                END;";
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", productId),
                new("@Quantity", quantity),
                new("@Date", date)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;

        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var isUsedQuery = "SELECT COUNT(*) FROM Order_Product WHERE ProductId = @ProductId";
                        using (var isUsedCommand = new SqlCommand(isUsedQuery, connection, transaction))
                        {
                            isUsedCommand.Parameters.AddWithValue("@ProductId", productId);
                            var count = (int)await isUsedCommand.ExecuteScalarAsync();

                            if (count > 0)
                            {
                                var softDeleteQuery = "UPDATE dbo.Products SET SoftDeleted = 1 WHERE Id = @Id";
                                using (var softDeleteCommand = new SqlCommand(softDeleteQuery, connection, transaction))
                                {
                                    softDeleteCommand.Parameters.AddWithValue("@Id", productId);
                                    var rowsAffected = await softDeleteCommand.ExecuteNonQueryAsync();
                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                var hardDeleteQuery = "DELETE FROM dbo.Products WHERE Id = @Id";
                                using (var hardDeleteCommand = new SqlCommand(hardDeleteQuery, connection, transaction))
                                {
                                    hardDeleteCommand.Parameters.AddWithValue("@Id", productId);
                                    var rowsAffected = await hardDeleteCommand.ExecuteNonQueryAsync();
                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        return true;
                                    }
                                }
                            }
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<List<string>> SearchProductsIdAsync(string searchTerm) {
            var query = @"
                SELECT
                    p.Id
                FROM
                    Products p
                    INNER JOIN CONTAINSTABLE(Products, (Name, Description), @SearchTerm) as Result
                    ON p.Id = Result.[Key]
                ORDER BY
                    Result.RANK DESC;";
            var productsId = new List<string>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", searchTerm);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                productsId.Add(reader["Id"].ToString());
            }
            return productsId;
        }
    }
}

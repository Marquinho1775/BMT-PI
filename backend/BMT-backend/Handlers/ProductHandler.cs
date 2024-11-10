using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BMT_backend.Domain.Entities;
using BMT_backend.Domain.Requests;
using BMT_backend.Domain.Views;

namespace BMT_backend.Handlers
{
    public class ProductHandler
    {
        private readonly string _connectionString;

        public ProductHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BMTContext");
        }

        private DataTable ExecuteQuery(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            using var adapter = new SqlDataAdapter(command);
            var resultTable = new DataTable();
            adapter.Fill(resultTable);
            return resultTable;
        }

        private int ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            connection.Open();
            return command.ExecuteNonQuery();
        }

        private object ExecuteScalar(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            connection.Open();
            return command.ExecuteScalar();
        }

        public string CreateProduct(Product product)
        {
            CreateBaseProduct(product);
            if (product.Type == "NonPerishable")
                CreateNonPerishableProduct(product);
            else if (product.Type == "Perishable")
                CreatePerishableProduct(product);
            CreateProductTags(product);
            return product.Id;
        }

        private void CreateBaseProduct(Product product)
        {
            const string query = @"
                INSERT INTO Products (EnterpriseId, Name, Description, Price, Weight)
                OUTPUT Inserted.Id
                VALUES (@EnterpriseId, @Name, @Description, @Price, @Weight);";
            var parameters = new List<SqlParameter>
            {
                new("@EnterpriseId", product.EnterpriseId),
                new("@Name", product.Name),
                new("@Description", product.Description),
                new("@Price", product.Price),
                new("@Weight", product.Weight)
            };
            product.Id = ExecuteScalar(query, parameters)?.ToString();
        }

        private void CreateNonPerishableProduct(Product product)
        {
            const string query = "INSERT INTO NonPerishableProducts (ProductId, Stock) VALUES (@ProductId, @Stock);";
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", product.Id),
                new("@Stock", product.Stock)
            };
            ExecuteNonQuery(query, parameters);
        }

        private void CreatePerishableProduct(Product product)
        {
            const string query = @"
                INSERT INTO PerishableProducts (ProductId, Limit, WeekDaysAvailable)
                VALUES (@ProductId, @Limit, @WeekDaysAvailable);";

            var parameters = new List<SqlParameter>
            {
                new("@ProductId", product.Id),
                new("@Limit", product.Limit),
                new("@WeekDaysAvailable", product.WeekDaysAvailable)
            };

            ExecuteNonQuery(query, parameters);
        }

        private void CreateProductTags(Product product)
        {
            const string query = @"
                INSERT INTO ProductTags (ProductId, TagId)
                VALUES (@ProductId, (SELECT Id FROM Tags WHERE Name = @TagName));";
            foreach (var tag in product.Tags)
            {
                var parameters = new List<SqlParameter>
                {
                    new("@ProductId", product.Id),
                    new("@TagName", tag)
                };
                ExecuteNonQuery(query, parameters);
            }
        }
        public List<Product> GetProducts()
        {
            const string query = "SELECT Id FROM Products;";
            var resultTable = ExecuteQuery(query);
            var products = new List<Product>();
            foreach (DataRow row in resultTable.Rows)
            {
                var product = GetProductById(row["Id"].ToString());
                if (product != null)
                    products.Add(product);
            }
            return products;
        }

        public Product GetProductById(string productId)
        {
            const string query = @"
                SELECT p.Id, p.Name, p.Description, p.Weight, p.Price, e.Id AS EnterpriseId, e.Name AS EnterpriseName
                FROM Products p
                JOIN Enterprises e ON p.EnterpriseId = e.Id
                WHERE p.Id = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            var resultTable = ExecuteQuery(query, parameters);
            var row = resultTable.Rows[0];
            var product = new Product
            {
                Id = row["Id"].ToString(),
                EnterpriseName = row["EnterpriseName"].ToString(),
                EnterpriseId = row["EnterpriseId"].ToString(),
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString(),
                Weight = Convert.ToDouble(row["Weight"]),
                Price = Convert.ToDouble(row["Price"]),
                Type = GetProductType(productId),
                Tags = GetProductTags(productId),
                ImagesURLs = GetProductImages(productId)
            };
            if (product.Type == "NonPerishable")
                product.Stock = GetNonPerishableStock(productId);
            else if (product.Type == "Perishable")
            {
                product.Limit = GetPerishableLimit(productId);
                product.WeekDaysAvailable = GetPerishableWeekDays(productId);
            }
            return product;
        }

        private string GetProductType(string productId)
        {
            const string query = @"
                SELECT
                    CASE
                        WHEN EXISTS (SELECT 1 FROM NonPerishableProducts WHERE ProductId = @ProductId) THEN 'NonPerishable'
                        WHEN EXISTS (SELECT 1 FROM PerishableProducts WHERE ProductId = @ProductId) THEN 'Perishable'
                        ELSE ''
                    END AS ProductType;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            return ExecuteScalar(query, parameters)?.ToString() ?? string.Empty;
        }

        private List<string> GetProductTags(string productId)
        {
            const string query = @"
                SELECT t.Name
                FROM Tags t
                JOIN ProductTags pt ON t.Id = pt.TagId
                WHERE pt.ProductId = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            var resultTable = ExecuteQuery(query, parameters);
            var tags = new List<string>();
            foreach (DataRow row in resultTable.Rows)
                tags.Add(row["Name"].ToString());
            return tags;
        }

        private List<string> GetProductImages(string productId)
        {
            const string query = @"
                SELECT i.URL
                FROM ProductImages i
                WHERE i.ProductId = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            var resultTable = ExecuteQuery(query, parameters);
            var images = new List<string>();
            foreach (DataRow row in resultTable.Rows)
                images.Add(row["URL"].ToString());
            return images;
        }

        private int GetNonPerishableStock(string productId)
        {
            const string query = "SELECT Stock FROM NonPerishableProducts WHERE ProductId = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            return Convert.ToInt32(ExecuteScalar(query, parameters));
        }

        private int GetPerishableLimit(string productId)
        {
            const string query = "SELECT Limit FROM PerishableProducts WHERE ProductId = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            return Convert.ToInt32(ExecuteScalar(query, parameters));
        }

        private string GetPerishableWeekDays(string productId)
        {
            const string query = "SELECT WeekDaysAvailable FROM PerishableProducts WHERE ProductId = @ProductId;";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            return ExecuteScalar(query, parameters)?.ToString();
        }

        public List<Product> GetProductsByEnterprise(string enterpriseName)
        {
            const string query = @"
                SELECT p.Id
                FROM Products p
                JOIN Enterprises e ON p.EnterpriseId = e.Id
                WHERE e.Name = @EnterpriseName;";
            var parameters = new List<SqlParameter> { new("@EnterpriseName", enterpriseName) };
            var resultTable = ExecuteQuery(query, parameters);
            var products = new List<Product>();
            foreach (DataRow row in resultTable.Rows)
            {
                var product = GetProductById(row["Id"].ToString());
                if (product != null)
                    products.Add(product);
            }
            return products;
        }

        public int GetStock(ProductStockRequest product)
        { 
            if (product.Type == "NonPerishable")
            {
                const string query = "SELECT Stock FROM NonPerishableProducts WHERE ProductId = @ProductId;";
                var parameters = new List<SqlParameter> { new("@ProductId", product.ProductId) };
                return Convert.ToInt32(ExecuteScalar(query, parameters));
            }
            else if (product.Type == "Perishable")
            {
                const string query = @"
                SELECT ISNULL(dd.Stock, pp.Limit) AS Stock
                FROM PerishableProducts pp
                LEFT JOIN DateDisponibility dd ON dd.ProductId = pp.ProductId AND dd.Date = @Date
                WHERE pp.ProductId = @ProductId;";
                var parameters = new List<SqlParameter>
                {
                    new("@Date", product.Date),
                    new("@ProductId", product.ProductId)
                };
                return Convert.ToInt32(ExecuteScalar(query, parameters));
            }
            else
            {
                throw new ArgumentException("Invalid product type or date.");
            }
        }

        public bool UpdateProduct(Product updatedProduct)
        {

            var updateQuery = "update Products set " +
                "Name = @Name, Description = @Description, Weight = @Weight, Price = @Price " +
                "where Id = @Id";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", updatedProduct.Id),
                new SqlParameter("@Name", updatedProduct.Name),
                new SqlParameter("@Description", updatedProduct.Description),
                new SqlParameter("@Weight", updatedProduct.Weight),
                new SqlParameter("@Price", updatedProduct.Price)
            };
            var rowsAffected = ExecuteNonQuery(updateQuery, parameters);
            if (rowsAffected == 0)
                return false;
            if (updatedProduct.Tags != null)
                UpdateProductTags(updatedProduct.Id, updatedProduct.Tags);
            if (updatedProduct.ImagesURLs != null && updatedProduct.ImagesURLs.Count > 0)
                UpdateProductImages(updatedProduct.Id, updatedProduct.ImagesURLs);
            UpdateProductTypeSpecificDetails(updatedProduct);
            return true;
        }

        private void UpdateProductTags(string productId, List<string> newTags)
        {
            const string deleteQuery = "DELETE FROM ProductTags WHERE ProductId = @ProductId;";
            var deleteParams = new List<SqlParameter> { new("@ProductId", productId) };
            ExecuteNonQuery(deleteQuery, deleteParams);
            const string insertQuery = @"
                INSERT INTO ProductTags (ProductId, TagId)
                VALUES (@ProductId, (SELECT Id FROM Tags WHERE Name = @TagName));";
            foreach (var tag in newTags)
            {
                var insertParams = new List<SqlParameter>
                {
                    new("@ProductId", productId),
                    new("@TagName", tag)
                };
                ExecuteNonQuery(insertQuery, insertParams);
            }
        }

        private void UpdateProductImages(string productId, List<string> newImageUrls)
        {
            const string deleteQuery = "DELETE FROM ProductImages WHERE ProductId = @ProductId;";
            var deleteParams = new List<SqlParameter> { new("@ProductId", productId) };
            ExecuteNonQuery(deleteQuery, deleteParams);
            const string insertQuery = @"
                INSERT INTO ProductImages (Id, ProductId, URL)
                VALUES (@Id, @ProductId, @URL);";
            foreach (var url in newImageUrls)
            {
                var insertParams = new List<SqlParameter>
                {
                    new("@Id", Guid.NewGuid().ToString()),
                    new("@ProductId", productId),
                    new("@URL", url)
                };
                ExecuteNonQuery(insertQuery, insertParams);
            }
        }

        private void UpdateProductTypeSpecificDetails(Product updatedProduct)
        {
            if (updatedProduct.Type == "NonPerishable" && updatedProduct.Stock.HasValue)
            {
                const string query = @"
                    UPDATE NonPerishableProducts
                    SET Stock = @Stock
                    WHERE ProductId = @ProductId;";
                var parameters = new List<SqlParameter>
                {
                    new("@Stock", updatedProduct.Stock.Value),
                    new("@ProductId", updatedProduct.Id)
                };
                ExecuteNonQuery(query, parameters);
            }
            else if (updatedProduct.Type == "Perishable")
            {
                var queryBuilder = new StringBuilder("UPDATE PerishableProducts SET ");
                var parameters = new List<SqlParameter>();
                if (updatedProduct.Limit.HasValue)
                {
                    queryBuilder.Append("Limit = @Limit, ");
                    parameters.Add(new SqlParameter("@Limit", updatedProduct.Limit.Value));
                }
                if (!string.IsNullOrEmpty(updatedProduct.WeekDaysAvailable))
                {
                    queryBuilder.Append("WeekDaysAvailable = @WeekDaysAvailable, ");
                    parameters.Add(new SqlParameter("@WeekDaysAvailable", updatedProduct.WeekDaysAvailable));
                }
                if (parameters.Count > 0)
                {
                    queryBuilder.Length -= 2;
                    queryBuilder.Append(" WHERE ProductId = @ProductId");
                    parameters.Add(new SqlParameter("@ProductId", updatedProduct.Id));
                    ExecuteNonQuery(queryBuilder.ToString(), parameters);
                }
            }
        }
        public string UpdateStock(string productId, int quantity, string dateString = "")
        {
            var type = GetProductType(productId);
            if (type == "NonPerishable")
                UpdateNonPerishableStock(productId, quantity);
            else if (type == "Perishable" && DateTime.TryParse(dateString, out var date))
                UpdatePerishableStock(productId, quantity, date);
            else
                throw new ArgumentException("Invalid product type or date.");
            return "Product stock updated successfully.";
        }

        private void UpdateNonPerishableStock(string productId, int quantity)
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
            ExecuteNonQuery(query, parameters);
        }

        private void UpdatePerishableStock(string productId, int quantity, DateTime date)
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
            ExecuteNonQuery(query, parameters);
        }
    }
}
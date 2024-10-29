using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using BMT_backend.Controllers;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System.Text;

namespace BMT_backend.Handlers
{
    public class ProductHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;
        private readonly ImageFileController _imageFileController;

        public ProductHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _conectionPath = builder.Configuration.GetConnectionString("BMTContext");
            _conection = new SqlConnection(_conectionPath);
        }

        private DataTable CreateQueryTable(string query)
        {
            SqlCommand queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            return tableFormatQuery;
        }

        public string CreateProduct(ProductModel product)
        {
            string productId = CreateBaseProduct(product);
            bool exit = AddTagsToProduct(productId, product.Tags);
            if (product.Type == "NonPerishable")
            {
                exit = CreateNonPerishableProduct(productId, product.Stock.Value);
            }
            else if(product.Type == "Perishable")
            {
                exit = CreatePerishableProduct(productId, product.Limit.Value, product.WeekDaysAvailable);
            }
            if (exit)
            {
                return productId;
            }
            return string.Empty;
        }

        private string CreateBaseProduct (ProductModel product)
        {
            string createProductQuery = "insert into Products (EnterpriseId, Name, Description, Price, Weight) " +
             "output Inserted.Id " +
             "values(@EnterpriseId, @Name, @Description, @Price, @Weight);";

            var createProductCommand = new SqlCommand(createProductQuery, _conection);
            createProductCommand.Parameters.AddWithValue("@EnterpriseId", product.EnterpriseId);
            createProductCommand.Parameters.AddWithValue("@Name", product.Name);
            createProductCommand.Parameters.AddWithValue("@Description", product.Description);
            createProductCommand.Parameters.AddWithValue("@Price", product.Price);
            createProductCommand.Parameters.AddWithValue("@Weight", product.Weight);

            _conection.Open();
            var productId = createProductCommand.ExecuteScalar()?.ToString();
            _conection.Close();
            return productId;
        }

        public bool CreateNonPerishableProduct(string ProductId, int Stock)
        {
            string createNonPerishableProductQuery = "insert into NonPerishableProducts (ProductId, Stock) " +
                "values(@ProductId, @Stock);";
            var createNonPerishableProductCommand = new SqlCommand(createNonPerishableProductQuery, _conection);
            createNonPerishableProductCommand.Parameters.AddWithValue("@ProductId", ProductId);
            createNonPerishableProductCommand.Parameters.AddWithValue("@Stock", Stock);
            _conection.Open();
            bool exit = createNonPerishableProductCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            return exit; 
        }

        public bool CreatePerishableProduct(string ProductId, int Limit, string WeekDaysAvailable)
        {
            string createPerishableProductQuery = "insert into PerishableProducts (ProductId, Limit, WeekDaysAvailable) " +
                "values(@ProductId, @Limit, @WeekDaysAvailable);";
            var createPerishableProductCommand = new SqlCommand(createPerishableProductQuery, _conection);
            createPerishableProductCommand.Parameters.AddWithValue("@ProductId", ProductId);
            createPerishableProductCommand.Parameters.AddWithValue("@Limit", Limit);
            createPerishableProductCommand.Parameters.AddWithValue("@WeekDaysAvailable", WeekDaysAvailable);
            _conection.Open();
            bool exit = createPerishableProductCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            return exit;
        }

        private bool CreateDateDisponibility(string ProductId, string WeekDaysAvailable)
        {
            string createDisponibilityQuery = "insert into DateDisponibility (ProductId, Date, Stock) " +
                "values(@ProductId, @Date, (select Limit from PerishableProducts where ProductId = @ProductId));";
            var createDisponibilityCommand = new SqlCommand(createDisponibilityQuery, _conection);
            createDisponibilityCommand.Parameters.AddWithValue("@ProductId", ProductId);
            createDisponibilityCommand.Parameters.Add("@Date", SqlDbType.Date);

            int[] dispatchDays = WeekDaysAvailable.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
            List<DateTime> dispatchDates = GetDispatchDates(dispatchDays);
            foreach (DateTime date in dispatchDates)
            {
                Console.WriteLine(date.ToString("dd-MM-yyyy"));
            }
            _conection.Open();
            foreach (DateTime date in dispatchDates)
            {
                createDisponibilityCommand.Parameters["@Date"].Value = date;
                createDisponibilityCommand.ExecuteNonQuery();
            }
            _conection.Close();
            return true;
        }

        private static List<DateTime> GetDispatchDates(int[] dispatchDays)
        {
            List<DateTime> dispatchDates = new List<DateTime>();
            DateTime startDate = DateTime.Now.Date;
            int currentDayNumber = (int)DateTime.Now.DayOfWeek;

            foreach (int day in dispatchDays.Distinct())
            {
                int daysToAdd = (day >= currentDayNumber) ? day - currentDayNumber : 7 - (currentDayNumber - day);
                DateTime futureDate = startDate.AddDays(daysToAdd);
                dispatchDates.Add(futureDate);
            }
            return dispatchDates.OrderBy(d => d).ToList();
        }

        private bool AddTagsToProduct(string ProductId, List<string> Tags)
        {
            string addTagsQuery = "insert into ProductTags (ProductId, TagId) " +
                "values(@ProductId, (select Id from Tags where Name = @TagName));";
            var addTagsCommand = new SqlCommand(addTagsQuery, _conection);
            addTagsCommand.Parameters.AddWithValue("@ProductId", ProductId);
            addTagsCommand.Parameters.Add("@TagName", SqlDbType.VarChar);

            _conection.Open();
            foreach (string tag in Tags)
            {
                addTagsCommand.Parameters["@TagName"].Value = tag;
                addTagsCommand.ExecuteNonQuery();
            }
            _conection.Close();
            return true;
        }

        public ProductModel GetProduct(string productId)
        {
            string query = "SELECT p.Id, p.Name, p.Description, p.Weight, p.Price, e.Id AS EnterpriseId " +
                           "FROM Products p " +
                           "JOIN Enterprises e ON p.EnterpriseId = e.Id " +
                           "WHERE p.Id = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable resultTable = new DataTable();
            _conection.Open();
            tableAdapter.Fill(resultTable);
            _conection.Close();
            DataRow row = resultTable.Rows[0];
            ProductModel product = new ProductModel
            {
                Id = Convert.ToString(row["Id"]),
                Name = Convert.ToString(row["Name"]),
                Description = Convert.ToString(row["Description"]),
                Weight = Convert.ToDouble(row["Weight"]),
                Price = Convert.ToDouble(row["Price"]),
                EnterpriseId = Convert.ToString(row["EnterpriseId"]),
                Tags = GetProductTags(productId),
                ImagesURLs = GetProductImages(productId),
                Type = GetProductType(productId),
            };
            if (product.Type == "NonPerishable")
            {
                product.Stock = GetNonPerishableStock(productId);
            }
            else if (product.Type == "Perishable")
            {
                product.Limit = GetPerishableLimit(productId);
                product.WeekDaysAvailable = GetPerishableWeekDays(productId);
            }
            return product;
        }

        private string GetProductType(string productId)
        {
            string query = "SELECT COUNT(*) FROM NonPerishableProducts WHERE ProductId = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int nonPerishableCount = (int)queryCommand.ExecuteScalar();
            _conection.Close();
            if (nonPerishableCount > 0)
            {
                return "NonPerishable";
            }
            query = "SELECT COUNT(*) FROM PerishableProducts WHERE ProductId = @productId";
            queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int perishableCount = (int)queryCommand.ExecuteScalar();
            _conection.Close();
            if (perishableCount > 0)
            {
                return "Perishable";
            }
            return string.Empty;
        }

        private int GetNonPerishableStock(string productId)
        {
            string query = "SELECT Stock FROM NonPerishableProducts WHERE ProductId = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int stock = (int)queryCommand.ExecuteScalar();
            _conection.Close();
            return stock;
        }

        private int GetPerishableLimit(string productId)
        {
            string query = "SELECT Limit FROM PerishableProducts WHERE ProductId = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            int limit = (int)queryCommand.ExecuteScalar();
            _conection.Close();
            return limit;
        }

        private string GetPerishableWeekDays(string productId)
        {
            string query = "SELECT WeekDaysAvailable FROM PerishableProducts WHERE ProductId = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            string weekDays = queryCommand.ExecuteScalar().ToString();
            _conection.Close();
            return weekDays;
        }

        public double GetProductPrice(string productId)
        {
            string query = "SELECT Price FROM Products WHERE Id = @productId";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@productId", productId);
            _conection.Open();
            double price = Convert.ToDouble(queryCommand.ExecuteScalar());
            _conection.Close();
            return price;
        }

        public List<ProductViewModel> GetProducts()
            {
            List<ProductViewModel> products = new List<ProductViewModel>();
            var query = "select p.Id, p.Name, p.Description, p.Weight, p.Price, e.Name as EnterpriseName " +
                "from Products p " +
                "join Enterprises e on p.EnterpriseId = e.Id;";
            DataTable table = CreateQueryTable(query);
            foreach (DataRow row in table.Rows)
            {
                products.Add(
                    new ProductViewModel
                    {
                        Id = Convert.ToString(row["Id"]),
                        Name = Convert.ToString(row["Name"]),
                        Description = Convert.ToString(row["Description"]),
                        Weight = Convert .ToDouble(row["Weight"]),
                        Price = Convert.ToDouble(row["Price"]),
                        EnterpriseName = Convert.ToString(row["EnterpriseName"]),
                        Tags = GetProductTags(Convert.ToString(row["Id"])),
                        ImagesURLs = GetProductImages(Convert.ToString(row["Id"]))
                    }
                );
            }
            return products;
        }

        private List<string> GetProductTags(string productId) {
            List<string> tags = new List<string>();
            string getTagsQuery = "select t.Name " +
                "from Tags t " +
                "join ProductTags pt on t.Id = pt.TagId " +
                "where pt.ProductId = @ProductId;";
            var getTagsCommand = new SqlCommand(getTagsQuery, _conection);
            getTagsCommand.Parameters.AddWithValue("@ProductId", productId);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(getTagsCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            foreach (DataRow row in tableFormatQuery.Rows)
            {
                tags.Add(row["Name"].ToString());
            }
            return tags;
        }

        private List<string> GetProductImages(string productId)
        {
            List<string> images = new List<string>();
            string getImagesQuery = "select i.URL " +
                "from ProductImages i " +
                "join Products p on i.ProductId = p.Id " +
                "where p.Id = @ProductId;";
            var getImagesCommand = new SqlCommand(getImagesQuery, _conection);
            getImagesCommand.Parameters.AddWithValue("@ProductId", productId);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(getImagesCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();
            foreach (DataRow row in tableFormatQuery.Rows)
            {
                images.Add(row["URL"].ToString());
            }
            return images;
        }

        public List<string> GetTags()
        {
            List<string> tags = new List<string>();
            var query = "select Name from Tags;";
            DataTable table = CreateQueryTable(query);
            foreach (DataRow row in table.Rows)
            {
                tags.Add(
                    Convert.ToString(row["Name"])
                );
            }
            return tags;
        }

        public List<DevProductModel> GetDevProducts()
        {
            List<DevProductModel> devProducts = new List<DevProductModel>();
            string query = "SELECT p.Name, p.Price, p.Description, e.Name AS Enterprise " +
               "FROM Products p " +
               "JOIN Enterprises e ON p.EnterpriseId = e.Id";

            var queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable resultTable = new DataTable();
            _conection.Open();
            tableAdapter.Fill(resultTable);
            _conection.Close();
            foreach (DataRow row in resultTable.Rows)
            {
                devProducts.Add(
                    new DevProductModel
                    {
                        Name = Convert.ToString(row["Name"]),
                        Enterprise = Convert.ToString(row["Enterprise"]),
                        Price = Convert.ToString(row["Price"]),
                        Description = Convert.ToString(row["Description"]),
                    });
            }
            return devProducts;
        }

        public string UpdateDateDisponibility(string PerishableProductId, string DateString, int Quantity)
        {
            DateTime Date = DateTime.Parse(DateString);
            string query = @"
            IF EXISTS (SELECT 1 FROM DateDisponibility WHERE ProductId = @ProductId AND Date = @Date)
            BEGIN
                -- Si existe, actualizar el Stock
                UPDATE DateDisponibility 
                SET Stock = Stock - @Quantity
                WHERE ProductId = @ProductId AND Date = @Date;
            END
            ELSE
            BEGIN
                -- Si no existe, insertar una nueva entrada con el Stock calculado
                INSERT INTO DateDisponibility (ProductId, Date, Stock) 
                VALUES (
                    @ProductId, 
                    @Date, 
                    (SELECT [Limit] FROM PerishableProducts WHERE ProductId = @ProductId) - @Quantity
                );
            END
            ";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@ProductId", PerishableProductId);
            queryCommand.Parameters.AddWithValue("@Date", Date);
            queryCommand.Parameters.AddWithValue("@Quantity", Quantity);
            _conection.Open();
            queryCommand.ExecuteNonQuery();
            _conection.Close();
            return "Date disponibility updated successfully.";
        }

        public List<ProductModel> GetProductsByEnterprise(string enterpriseName)
        {
            List<ProductModel> productsOfEnterprise = new List<ProductModel>();

            var query = "SELECT p.Id, p.Name, p.Description, p.Weight, p.Price, e.Name as EnterpriseName " +
                        "FROM Products p " +
                        "JOIN Enterprises e ON p.EnterpriseId = e.Id " +
                        "WHERE e.Name = @EnterpriseName;";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@enterpriseName", enterpriseName);

            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                var productId = Convert.ToString(row["Id"]);
                var productType = GetProductType(productId);

                var product = new ProductModel
                {
                    Id = productId,
                    Name = Convert.ToString(row["Name"]),
                    Description = Convert.ToString(row["Description"]),
                    Weight = Convert.ToDouble(row["Weight"]),
                    Price = Convert.ToDouble(row["Price"]),
                    Tags = GetProductTags(productId),
                    ImagesURLs = GetProductImages(productId),
                    Type = productType
                };
                if (productType == "NonPerishable")
                {
                    product.Stock = GetNonPerishableStock(productId);
                }
                else if (productType == "Perishable")
                {
                    product.Limit = GetPerishableLimit(productId);
                    product.WeekDaysAvailable = GetPerishableWeekDays(productId);
                }

                productsOfEnterprise.Add(product);
            }
            return productsOfEnterprise;
        }

        private void UpdateProductTags(string productId, List<string> newTags)
        {
            var deleteQuery = "DELETE FROM ProductTags WHERE ProductId = @ProductId";
            using (var deleteCommand = new SqlCommand(deleteQuery, _conection))
            {
                deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                _conection.Open();
                deleteCommand.ExecuteNonQuery();
                _conection.Close();
            }

            var insertQuery = "INSERT INTO ProductTags (ProductId, TagId) VALUES (@ProductId, @TagId)";
            foreach (var tagId in newTags)
            {
                using (var insertCommand = new SqlCommand(insertQuery, _conection))
                {
                    insertCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertCommand.Parameters.AddWithValue("@TagId", tagId);
                    _conection.Open();
                    insertCommand.ExecuteNonQuery();
                    _conection.Close();
                }
            }
        }

        private void UpdateProductImages(string productId, List<string> newImageUrls)
        {
            var deleteQuery = "DELETE FROM ProductImages WHERE ProductId = @ProductId";
            using (var deleteCommand = new SqlCommand(deleteQuery, _conection))
            {
                deleteCommand.Parameters.AddWithValue("@ProductId", productId);
                _conection.Open();
                deleteCommand.ExecuteNonQuery();
                _conection.Close();
            }
            var insertQuery = "INSERT INTO ProductImages (Id, ProductId, URL) VALUES (@Id, @ProductId, @URL)";
            foreach (var url in newImageUrls)
            {
                using (var insertCommand = new SqlCommand(insertQuery, _conection))
                {
                    insertCommand.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                    insertCommand.Parameters.AddWithValue("@ProductId", productId);
                    insertCommand.Parameters.AddWithValue("@URL", url);
                    _conection.Open();
                    insertCommand.ExecuteNonQuery();
                    _conection.Close();
                }
            }
        }

        public bool UpdateProduct(ProductModel updatedProduct)
        {
            var query = new StringBuilder("UPDATE Products SET ");
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(updatedProduct.Name))
            {
                query.Append("Name = @Name, ");
                parameters.Add(new SqlParameter("@Name", updatedProduct.Name));
            }
            if (!string.IsNullOrEmpty(updatedProduct.Description))
            {
                query.Append("Description = @Description, ");
                parameters.Add(new SqlParameter("@Description", updatedProduct.Description));
            }
            if (updatedProduct.Weight != null)
            {
                query.Append("Weight = @Weight, ");
                parameters.Add(new SqlParameter("@Weight", updatedProduct.Weight));
            }
            if (updatedProduct.Price != null)
            {
                query.Append("Price = @Price, ");
                parameters.Add(new SqlParameter("@Price", updatedProduct.Price));
            }

            if (updatedProduct.Tags != null)
            {
                UpdateProductTags(updatedProduct.Id, updatedProduct.Tags);
            }


            if (updatedProduct.ImagesURLs != null && updatedProduct.ImagesURLs.Count > 0)
            {
                UpdateProductImages(updatedProduct.Id, updatedProduct.ImagesURLs);
            }


            query.Length -= 2;
            query.Append(" WHERE Id = @Id AND EnterpriseId = @EnterpriseId");
            parameters.Add(new SqlParameter("@Id", updatedProduct.Id));
            parameters.Add(new SqlParameter("@EnterpriseId", updatedProduct.EnterpriseId));

            using (var command = new SqlCommand(query.ToString(), _conection))
            {
                command.Parameters.AddRange(parameters.ToArray());
                _conection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                _conection.Close();

                if (rowsAffected == 0)
                {
                    return false;
                }
            }

            if (updatedProduct.Type == "NonPerishable" && updatedProduct.Stock.HasValue)
            {
                var updateNonPerishableQuery = "UPDATE NonPerishableProducts SET Stock = @Stock WHERE ProductId = @ProductId";
                using (var stockCommand = new SqlCommand(updateNonPerishableQuery, _conection))
                {
                    stockCommand.Parameters.AddWithValue("@Stock", updatedProduct.Stock.Value);
                    stockCommand.Parameters.AddWithValue("@ProductId", updatedProduct.Id);
                    _conection.Open();
                    stockCommand.ExecuteNonQuery();
                    _conection.Close();
                }
            }
            else if (updatedProduct.Type == "Perishable")
            {
                var updatePerishableQuery = new StringBuilder("UPDATE PerishableProducts SET ");
                var perishableParameters = new List<SqlParameter>();

                if (updatedProduct.Limit.HasValue)
                {
                    updatePerishableQuery.Append("Limit = @Limit, ");
                    perishableParameters.Add(new SqlParameter("@Limit", updatedProduct.Limit.Value));
                }
                if (!string.IsNullOrEmpty(updatedProduct.WeekDaysAvailable))
                {
                    updatePerishableQuery.Append("WeekDaysAvailable = @WeekDaysAvailable, ");
                    perishableParameters.Add(new SqlParameter("@WeekDaysAvailable", updatedProduct.WeekDaysAvailable));
                }

                if (perishableParameters.Count > 0)
                {
                    updatePerishableQuery.Length -= 2;
                    updatePerishableQuery.Append(" WHERE ProductId = @ProductId");
                    perishableParameters.Add(new SqlParameter("@ProductId", updatedProduct.Id));

                    using (var perishableCommand = new SqlCommand(updatePerishableQuery.ToString(), _conection))
                    {
                        perishableCommand.Parameters.AddRange(perishableParameters.ToArray());
                        _conection.Open();
                        perishableCommand.ExecuteNonQuery();
                        _conection.Close();
                    }
                }
            }

            return true;
        }

        public List<string> GetTagsIDBasedOnProductID(string productId)
        {
            List<string> tagsId = new List<string>();

            string getTagsQuery = "SELECT pt.TagId " +
                "FROM ProductTags pt " +
                "WHERE pt.ProductId = @ProductId;";
            var getTagsCommand = new SqlCommand(getTagsQuery, _conection);
            getTagsCommand.Parameters.AddWithValue("@ProductId", productId);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(getTagsCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                tagsId.Add(row["TagId"].ToString());
            }

            return tagsId;
        }

        public List<string> GetTagsIDBasedOnTagName(string tagName)
        {
            List<string> tagsId = new List<string>();

            string getTagsQuery = "SELECT Id " +
                "FROM Tags " +
                "WHERE Name = @tagName;";
            var getTagsCommand = new SqlCommand(getTagsQuery, _conection);
            getTagsCommand.Parameters.AddWithValue("@tagName", tagName);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(getTagsCommand);
            DataTable tableFormatQuery = new DataTable();
            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                tagsId.Add(row["Id"].ToString());
            }

            return tagsId;
        }

        public string UpdateStock(string id, int newStock)
        {
            string updateStockQuery = "update NonPerishableProducts set Stock = @newStock where ProductId = @id";
            var updateStockCommand = new SqlCommand(updateStockQuery, _conection);
            updateStockCommand.Parameters.AddWithValue("@newStock", newStock);
            updateStockCommand.Parameters.AddWithValue("@id", id);
            _conection.Open();
            int rowsAffected = updateStockCommand.ExecuteNonQuery();
            _conection.Close();
            if (rowsAffected > 0)
            {
                return "Stock updated successfully.";
            }
            return "Error updating stock.";
        }

        public int GetStock(string id)
        {
            string query = "select Stock from NonPerishableProducts where ProductId = @id";
            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@id", id);
            _conection.Open();
            int stock = (int)queryCommand.ExecuteScalar();
            _conection.Close();
            return stock;
        }

        public int GetStockPerishable(string Id, string Date)
        {
            string query = @"
        SELECT ISNULL(dd.Stock, pp.Limit) AS Stock
        FROM PerishableProducts pp
        LEFT JOIN DateDisponibility dd ON dd.ProductId = pp.ProductId AND dd.Date = @Date
        WHERE pp.ProductId = @Id";

            var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@Date", Date);
            queryCommand.Parameters.AddWithValue("@Id", Id);

            _conection.Open();
            int stock = (int)queryCommand.ExecuteScalar();
            _conection.Close();

            return stock;
        }
    }
}
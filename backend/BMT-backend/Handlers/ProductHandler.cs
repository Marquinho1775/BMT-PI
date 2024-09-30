using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BMT_backend.Handlers
{
    public class ProductHandler
    {
        private SqlConnection _conection;
        private string _conectionPath;

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
            string createProductQuery = "insert into Products (EnterpriseId, Name, Description, Price, Weight) " +
                "output Inserted.Id " +
                "values((select Id from Enterprises where IdentificationNumber = @EnterpriseIdentification), " +
                "@Name, @Description, @Price, @Weight);";

            var createProductCommand = new SqlCommand(createProductQuery, _conection);
            createProductCommand.Parameters.AddWithValue("@EnterpriseIdentification", GetEnterpriseIdByUsername(product.Username));
            createProductCommand.Parameters.AddWithValue("@Name", product.Name);
            createProductCommand.Parameters.AddWithValue("@Description", product.Description);
            createProductCommand.Parameters.AddWithValue("@Price", product.Price);
            createProductCommand.Parameters.AddWithValue("@Weight", product.Weight);

            _conection.Open();
            var productId = createProductCommand.ExecuteScalar()?.ToString();
            _conection.Close();
            return productId ?? string.Empty;
        }

        private string GetEnterpriseIdByUsername(string username)
        {
            string getEnterpriseIdFromUernameQuery = "SELECT e.Id " +
                "FROM Enterprises e " +
                "JOIN Entrepreneurs_Enterprises ee ON e.Id = ee.EnterpriseId " +
                "JOIN Entrepreneurs en ON ee.EntrepreneurId = en.Id " +
                "JOIN Users u ON en.UserId = u.Id " +
                "WHERE u.UserName = @UserName;";
            var getEnterpriseIdFromUernameCommand = new SqlCommand(getEnterpriseIdFromUernameQuery, _conection);
            getEnterpriseIdFromUernameCommand.Parameters.AddWithValue("@UserName", username);
            _conection.Open();
            var enterpriseIdentification = getEnterpriseIdFromUernameCommand.ExecuteScalar()?.ToString();
            _conection.Close();
            return enterpriseIdentification ?? string.Empty;
        }

        public bool CreateNonPerishableProduct(NonPerishableProductModel product)
        {
            string createNonPerishableProductQuery = "insert into NonPerishableProducts (ProductId, Stock) " +
                "values(@ProductId, @Stock);";
            var createNonPerishableProductCommand = new SqlCommand(createNonPerishableProductQuery, _conection);
            createNonPerishableProductCommand.Parameters.AddWithValue("@ProductId", product.ProductId);
            createNonPerishableProductCommand.Parameters.AddWithValue("@Stock", product.Stock);
            _conection.Open();
            bool exit = createNonPerishableProductCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            return exit; 

        }

        public bool CreatePerishableProduct(PerishableProductModel product)
        {
            string createPerishableProductQuery = "insert into PerishableProducts (ProductId, Stock, WeekDaysAvailable, DayLimitAvailable) " +
                "values(@ProductId, @Stock, @WeekDaysAvailable, @DayLimitAvailable);";
            var createPerishableProductCommand = new SqlCommand(createPerishableProductQuery, _conection);
            createPerishableProductCommand.Parameters.AddWithValue("@ProductId", product.ProductId);
            createPerishableProductCommand.Parameters.AddWithValue("@Stock", product.Stock);
            createPerishableProductCommand.Parameters.AddWithValue("@WeekDaysAvailable", product.WeekDaysAvailable);
            createPerishableProductCommand.Parameters.AddWithValue("@DayLimitAvailable", product.DayLimitAvailable);
            _conection.Open();
            bool exit = createPerishableProductCommand.ExecuteNonQuery() >= 1;
            _conection.Close();
            bool disponibilityCreated = CreateDateDisponibility(product);
            if (!disponibilityCreated)
            {
                return false;
            }
            return exit;
        }

        private bool CreateDateDisponibility(PerishableProductModel model) {            
            string createDisponibilityQuery = "insert into DateDisponibility (ProductId, Date, Stock) " +
                "values(@ProductId, @Date, (select Limit from PerishableProducts where ProductId = @ProductId));";
            var createDisponibilityCommand = new SqlCommand(createDisponibilityQuery, _conection);
            createDisponibilityCommand.Parameters.AddWithValue("@ProductId", model.ProductId);
            int[] dispatchDays = model.WeekDaysAvailable.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
            List<DateTime> dispatchDates = GetDispatchDates(dispatchDays);
            _conection.Open();
            foreach (DateTime date in dispatchDates)
            {
                createDisponibilityCommand.Parameters.AddWithValue("@Date", date);
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

        public bool AddTagsToProduct(AddTagsToProductRequest request)
        {
            string addTagsQuery = "insert into Product_Tags (ProductId, TagId) " +
                "values(@ProductId, (select Id from Tags where Name = @TagName));";
            var addTagsCommand = new SqlCommand(addTagsQuery, _conection);
            addTagsCommand.Parameters.AddWithValue("@ProductId", request.ProductId);
            _conection.Open();
            foreach (string tag in request.Tags)
            {
                addTagsCommand.Parameters.AddWithValue("@TagName", tag);
                addTagsCommand.ExecuteNonQuery();
            }
            _conection.Close();
            return true;
        }

        public List<ProductViewModel> GetProducts()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            var query = "select p.Id, p.Name, p.Description, p.Weight, p.Price, p.Type, e.Name as EnterpriseName " +
                "from Products p " +
                "join Enterprises e on p.EnterpriseId = e.Id;";
            DataTable table = CreateQueryTable(query);
            foreach (DataRow row in table.Rows)
            {
                products.Add(
                    new ProductViewModel
                    {
                        Name = Convert.ToString(row["Name"]),
                        Description = Convert.ToString(row["Description"]),
                        Weight = Convert.ToDouble(row["Weight"]),
                        Price = Convert.ToDouble(row["Price"]),
                        Type = Convert.ToBoolean(row["Type"]),
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
                "join Product_Tags pt on t.Id = pt.TagId " +
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
                "from Images i " +
                "join Product_Images pi on i.Id = pi.ImageId " +
                "where pi.ProductId = @ProductId;";
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
    }
}

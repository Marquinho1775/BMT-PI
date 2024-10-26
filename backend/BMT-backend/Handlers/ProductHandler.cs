using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using BMT_backend.Controllers;
using System.Data.SqlClient;

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

            bool disponibilityCreated = CreateDateDisponibility(ProductId, WeekDaysAvailable);
            if (!disponibilityCreated)
            {
                return false;
            }
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

        public List<ProductViewModel> GetProductsByEnterprise(string enterpriseName)
        {
            List<ProductViewModel> productsOfEnterprise = new List<ProductViewModel>();

            var query = "select p.Id, p.Name, p.Description, p.Weight, p.Price, e.Name as EnterpriseName " +
                        "from Products p " +
                        "join Enterprises e on p.EnterpriseId = e.Id " +
                        "where e.Name = @EnterpriseName;";
            var queryCommand = new SqlCommand(query, _conection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            queryCommand.Parameters.AddWithValue("@enterpriseName", enterpriseName);

            _conection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _conection.Close();

            foreach (DataRow row in tableFormatQuery.Rows)
            {
                productsOfEnterprise.Add(new ProductViewModel
                {
                    Id = Convert.ToString(row["Id"]),
                    Name = Convert.ToString(row["Name"]),
                    Description = Convert.ToString(row["Description"]),
                    Weight = Convert.ToDouble(row["Weight"]),
                    Price = Convert.ToDouble(row["Price"]),
                    EnterpriseName = Convert.ToString(row["EnterpriseName"]),
                    Tags = GetProductTags(Convert.ToString(row["Id"])),
                    ImagesURLs = GetProductImages(Convert.ToString(row["Id"]))
                });
            }
            return productsOfEnterprise;
        }
    }
}
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using BMT_backend.Models;
using System.Data.SqlClient;

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

        public List<DevProductModel> GetDevProducts() {
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
    }
}

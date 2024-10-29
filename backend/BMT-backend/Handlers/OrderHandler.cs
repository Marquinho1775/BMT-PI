using BMT_backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class OrderHandler
    {
        private SqlConnection _connection;

        private string _connectionString;

        public OrderHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("BMTContext");
            Connection = new SqlConnection(_connectionString);
        }
        public SqlConnection Connection { get => _connection; set => _connection = value; }

        private DataTable CreateQueryTable(string query, SqlParameter[] parameters = null)
        {
            DataTable tableFormatQuery = new DataTable();

            try
            {
                using (SqlCommand queryCommand = new SqlCommand(query, Connection))
                {
                    if (parameters != null)
                    {
                        queryCommand.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand))
                    {
                        Connection.Open();
                        tableAdapter.Fill(tableFormatQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateQueryTable: {ex.Message}");
            }
            finally
            {
                Connection.Close();
            }

            return tableFormatQuery;
        }
        public List<OrderModel> GetToConfirmOrders()
        {
            List<OrderModel> toConfirmOrders = new List<OrderModel>();
            string query = "SELECT * FROM dbo.Orders WHERE Status = 0";
            DataTable resultTable = CreateQueryTable(query);
            foreach (DataRow row in resultTable.Rows)
            {
                OrderModel order = new OrderModel();
                order.OrderId = row["OrderId"].ToString();
                order.OrderDate = Convert.ToDateTime(row["OrderDate"]);
                order.OrderCost = Convert.ToDecimal(row["OrderCost"]);
                order.DeliveryFee = Convert.ToDecimal(row["DeliveryFee"]);
                order.Weight = Convert.ToDecimal(row["Weight"]);
                order.UserId = row["UserId"].ToString();
                order.UserName = row["UserName"].ToString();
                order.Direction = row["Direction"].ToString();
                order.OtherSigns = row["OtherSigns"].ToString();
                order.Coordinates = row["Coordinates"].ToString();
                order.Status = Convert.ToInt32(row["Status"]);
                toConfirmOrders.Add(order);
            }
            return toConfirmOrders;
        }
        public bool ConfirmOrder(string orderId)
        {
            string query = "UPDATE dbo.Orders SET Status = 1 WHERE OrderId = @OrderId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            DataTable resultTable = CreateQueryTable(query, parameters);
            return resultTable.Rows.Count > 0;
        }
        public OrderModel GetOrderById(String orderId)
        {
            string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId, 
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email, d.Coordinates, o.Status
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.OrderId = @OrderId;";

            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            _connection.Open();
            using var reader = command.ExecuteReader();
            OrderModel order = null;

            if (reader.Read())
            {
                order = new OrderModel
                {
                    OrderId = reader["OrderId"].ToString(),
                    OrderDate = (DateTime)reader["OrderDate"],
                    OrderCost = (decimal)reader["OrderCost"],
                    DeliveryFee = (decimal)reader["DeliveryFee"],
                    Weight = (decimal)reader["Weight"],
                    UserId = reader["UserId"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    Direction = reader["NumDirection"].ToString(),
                    OtherSigns = reader["OtherSigns"].ToString(),
                    UserEmail = reader["Email"].ToString(),
                    Coordinates = reader["Coordinates"].ToString(),
                    Status = (int)reader["Status"],
                    Products = GetProductsByOrderId(orderId)
                };
            }

            _connection.Close();
            return order;
        }

        private List<ProductDetails> GetProductsByOrderId(String orderId)
        {
            var products = new List<ProductDetails>();
            string query = "SELECT op.ProductId, p.Name AS ProductName, op.Amount, op.ProductsCost, e.Name AS EnterpriseName, e.Email, op.DeliveryDate " +
                           "FROM Order_Product op " +
                           "JOIN Products p ON op.ProductId = p.Id " +
                           "JOIN Enterprises e ON p.EnterpriseId = e.Id" +
                           "WHERE op.OrderId = @OrderId;";

            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            _connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new ProductDetails
                {
                    ProductId = reader["ProductId"].ToString(),
                    ProductName = reader["ProductName"].ToString(),
                    Quantity = (int)reader["Amount"],
                    ProductsCost = (int)reader["ProductsCost"],
                    EnterpriseName = reader["EnterpriseName"].ToString(),
                    EnterpriseEmail = reader["Email"].ToString(),
                    ProductDate = (DateTime)reader["DeliveryDate"]
                });
            }

            _connection.Close();
            return products;
        }
    }
}

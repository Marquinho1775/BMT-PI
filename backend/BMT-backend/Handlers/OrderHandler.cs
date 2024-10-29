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
            var orders = new List<OrderModel>();
            string query = @"
            SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
               u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
               d.Coordinates, o.Status
            FROM Orders o
            JOIN Users u ON o.UserId = u.Id
            JOIN Directions d ON o.DirectionId = d.Id
            WHERE o.Status = 0;";

            using (var command = new SqlCommand(query, Connection))
            {
                Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new OrderModel
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
                            UserEmail = reader["UserEmail"].ToString(),
                            Coordinates = reader["Coordinates"].ToString(),
                            Status = (int)reader["Status"],
                            Products = GetProductsByOrderId(reader["OrderId"].ToString())
                        };
                        orders.Add(order);
                    }
                }
            }
            Connection.Close();
            return orders;
        }
        public bool ConfirmOrder(string orderId)
        {
            string query = "UPDATE dbo.Orders SET Status = 1 WHERE OrderId = @OrderId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
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

            OrderModel order = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        order = new OrderModel
                        {
                            OrderId = reader["OrderId"].ToString(),
                            OrderDate = reader["OrderDate"] as DateTime? ?? DateTime.MinValue,
                            OrderCost = reader["OrderCost"] as decimal? ?? 0,
                            DeliveryFee = reader["DeliveryFee"] as decimal? ?? 0,
                            Weight = reader["Weight"] as decimal? ?? 0,
                            UserId = reader["UserId"].ToString(),
                            UserName = reader["UserName"]?.ToString(),
                            Direction = reader["NumDirection"]?.ToString(),
                            OtherSigns = reader["OtherSigns"]?.ToString(),
                            UserEmail = reader["Email"]?.ToString(),
                            Coordinates = reader["Coordinates"]?.ToString(),
                            Status = reader["Status"] as int? ?? 0,
                            Products = GetProductsByOrderId(orderId)
                        };
                    }
                }
            }
            return order;
        }

        private List<ProductDetails> GetProductsByOrderId(String orderId)
        {
            var products = new List<ProductDetails>();
            string query = @"
            SELECT op.ProductId, p.Name AS ProductName, op.Amount, op.ProductsCost, 
               e.Name AS EnterpriseName, e.Email, op.DeliveryDate
            FROM Order_Product op
            JOIN Products p ON op.ProductId = p.Id
            JOIN Enterprises e ON p.EnterpriseId = e.Id
            WHERE op.OrderId = @OrderId;";

            using var command = new SqlCommand(query, Connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            Connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var product = new ProductDetails
                {
                    ProductId = reader["ProductId"].ToString(),
                    ProductName = reader["ProductName"].ToString(),
                    Quantity = Convert.ToInt32(reader["Amount"]),
                    ProductsCost = Convert.ToInt32(reader["ProductsCost"]),
                    EnterpriseName = reader["EnterpriseName"].ToString(),
                    EnterpriseEmail = reader["Email"].ToString(),
                    ProductDate = Convert.ToDateTime(reader["DeliveryDate"])
                };
                products.Add(product);
            }

            Connection.Close();
            return products;
        }

        public bool DenyOrder(string orderId)
        {
            string query = "UPDATE dbo.Orders SET Status = 5 WHERE OrderId = @OrderId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        public List<OrderModel> GetToConfirmUserOrders(String userId)
        {
            var orders = new List<OrderModel>();
            string query = @"
            SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
           u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
           d.Coordinates, o.Status
            FROM Orders o
            JOIN Users u ON o.UserId = u.Id
            JOIN Directions d ON o.DirectionId = d.Id
            WHERE o.Status = 0 AND o.UserId = @UserId;";

            using (var command = new SqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new OrderModel
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
                            UserEmail = reader["UserEmail"].ToString(),
                            Coordinates = reader["Coordinates"].ToString(),
                            Status = (int)reader["Status"],
                            Products = GetProductsByOrderId(reader["OrderId"].ToString())
                        };
                        orders.Add(order);
                    }
                }
                Connection.Close();
            }
            return orders;
        }
    }
}

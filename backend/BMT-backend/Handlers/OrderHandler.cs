using BMT_backend.Models;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Handlers
{
    public class OrderHandler
    {
        private readonly string _connectionString;

        public OrderHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _connectionString = builder.Configuration.GetConnectionString("BMTContext") + ";MultipleActiveResultSets=True";
        }

        private DataTable CreateQueryTable(string query, SqlParameter[] parameters = null)
        {
            var tableFormatQuery = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    connection.Open();
                    adapter.Fill(tableFormatQuery);
                }
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

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
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
                return command.ExecuteNonQuery() > 0;
            }
        }

        public OrderModel GetOrderById(string orderId)
        {
            string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId, 
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email, d.Coordinates, o.Status
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.OrderId = @OrderId;";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new OrderModel
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
                }
            }
            return null;
        }

        private List<ProductDetails> GetProductsByOrderId(string orderId)
        {
            var products = new List<ProductDetails>();
            string query = @"
                SELECT op.ProductId, p.Name AS ProductName, op.Amount, op.ProductsCost, 
                       e.Name AS EnterpriseName, e.Email, op.DeliveryDate
                FROM Order_Product op
                JOIN Products p ON op.ProductId = p.Id
                JOIN Enterprises e ON p.EnterpriseId = e.Id
                WHERE op.OrderId = @OrderId;";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
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
                }
            }
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
                return command.ExecuteNonQuery() > 0;
            }
        }

        public List<OrderModel> GetToConfirmUserOrders(string userId)
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

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
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
            return orders;
        }
    }
}

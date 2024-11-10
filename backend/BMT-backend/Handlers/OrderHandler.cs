using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;
using BMT_backend.Domain.Requests;
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

        public List<Order> GetOrders()
        {
            var orders = new List<Order>();
            string query = "SELECT * FROM Orders";
            var table = CreateQueryTable(query);

            foreach (DataRow row in table.Rows)
            {
                orders.Add(new Order
                {
                    OrderId = row["OrderId"].ToString(),
                    UserId = row["UserId"].ToString(),
                    DirectionId = row["DirectionId"].ToString(),
                    PaymentMethod = row["OrderPaymentMethod"].ToString(),
                    Status = (int)row["Status"],
                    DeliveryDate = row["OrderDeliveryDate"].ToString(),
                    OrderCost = Convert.ToDouble(row["OrderCost"]),
                    Weight = Convert.ToDouble(row["Weight"]),
                    DeliveryFee = Convert.ToDouble(row["DeliveryFee"])
                });
            }
            return orders;
        }

        public string CreateOrder(Order order)
        {
            string query = @"
                INSERT INTO Orders (UserId, DirectionId, OrderPaymentMethod, Status, OrderDeliveryDate, OrderCost, Weight, DeliveryFee)
                OUTPUT INSERTED.OrderId
                VALUES (@UserId, @DirectionId, @PaymentMethod, @Status, @DeliveryDate, 0, 0, 0);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", order.UserId);
                command.Parameters.AddWithValue("@DirectionId", order.DirectionId);
                command.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);
                command.Parameters.AddWithValue("@DeliveryDate", order.DeliveryDate);
                command.Parameters.AddWithValue("@Status", order.Status);
                connection.Open();
                return command.ExecuteScalar().ToString();
            }
        }

        public bool AddProductToOrder(AddProductToOrderRequest orderProduct)
        {
            string query = @"
                INSERT INTO Order_Product (OrderId, ProductId, Amount, ProductsCost)
                VALUES (@OrderId, @ProductId, @Amount, @ProductsCost);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@OrderId", orderProduct.OrderId);
                command.Parameters.AddWithValue("@ProductId", orderProduct.ProductId);
                command.Parameters.AddWithValue("@Amount", orderProduct.Amount);
                command.Parameters.AddWithValue("@ProductsCost", orderProduct.ProductsCost);
                connection.Open();
                command.ExecuteNonQuery();
            }

            double productWeight = 0;
            string getProductWeightQuery = "SELECT Weight FROM Products WHERE Id = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(getProductWeightQuery, connection))
            {
                command.Parameters.AddWithValue("@ProductId", orderProduct.ProductId);
                connection.Open();
                productWeight = Convert.ToDouble(command.ExecuteScalar());
            }
            double totalWeight = orderProduct.Amount * productWeight;

            string query2 = @"
                UPDATE Orders
                SET OrderCost = OrderCost + @ProductsCost, Weight = Weight + @totalWeight
                WHERE OrderId = @OrderId;";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query2, connection))
            {
                command.Parameters.AddWithValue("@ProductsCost", orderProduct.ProductsCost);
                command.Parameters.AddWithValue("@totalWeight", totalWeight);
                command.Parameters.AddWithValue("@OrderId", orderProduct.OrderId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateDeliverFee(string orderId)
        {
            string query = "SELECT DirectionId, Weight FROM Orders WHERE OrderId = @OrderId";
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            string directionId = reader["DirectionId"].ToString();
            double weight = Convert.ToDouble(reader["Weight"]);
            double deliveryFee = CalculateDeliveryFee(directionId, weight);
            string query2 = "UPDATE Orders SET DeliveryFee = @DeliveryFee WHERE OrderId = @OrderId";
            var connection2 = new SqlConnection(_connectionString);
            var command2 = new SqlCommand(query2, connection2);
            command2.Parameters.AddWithValue("@DeliveryFee", deliveryFee);
            command2.Parameters.AddWithValue("@OrderId", orderId);
            connection2.Open();
            return command2.ExecuteNonQuery() > 0;
        }

        public double CalculateDeliveryFee(string directionId, double weight)
        {
            string query = "select Coordinates from Directions where Id = @directionId";
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@directionId", directionId);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            string coordinates = reader["Coordinates"].ToString();
            string[] coord = coordinates.Split(',');
            double x = Convert.ToDouble(coord[0]);
            double y = Convert.ToDouble(coord[1]);
            double distance =  DistanceCalculator.CalcularDistancia(x, y);
            double deliveryFee = distance >= 25? 3000 : 2000;
            Console.WriteLine("Distancia: " + distance);
            Console.WriteLine("Delivery Fee: " + deliveryFee);
            deliveryFee += weight * 200;
            Console.WriteLine("Delivery Fee: " + deliveryFee);
            return deliveryFee;
        }

        public List<Order> GetToConfirmOrders()
        {
            var orders = new List<Order>();
            string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                d.Coordinates, o.Status, d.Id as DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate, o.OrderCost
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
                        var order = new Order
                        {
                            OrderId = reader["OrderId"].ToString(),
                            OrderDate = (DateTime)reader["OrderDate"],
                            Weight = Convert.ToDouble(reader["Weight"]),
                            DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                            UserId = reader["UserId"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Direction = reader["NumDirection"].ToString(),
                            DirectionId = reader["DirectionId"].ToString(),
                            PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                            DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                            OrderCost = Convert.ToDouble(reader["OrderCost"]),
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

        public Order GetOrderById(string orderId)
        {
            string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.OrderDeliveryDate, o.Weight, o.UserId, 
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
                        return new Order
                        {
                            OrderId = reader["OrderId"].ToString(),
                            OrderDate = (DateTime)reader["OrderDate"],
                            DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                            DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                            Weight = (double)reader["Weight"],
                            UserId = reader["UserId"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            OrderCost = Convert.ToDouble(reader["OrderCost"]),
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
                       e.Name AS EnterpriseName, e.Email
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

        public List<Order> GetToConfirmUserOrders(string userId)
        {
            var orders = new List<Order>();
            string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.OrderDeliveryDate, o.Weight, o.UserId,
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
                        var order = new Order
                        {
                            OrderId = reader["OrderId"].ToString(),
                            OrderDate = (DateTime)reader["OrderDate"],
                            Weight = Convert.ToDouble(reader["Weight"]),
                            DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                            DeliveryDate = reader["OrderDeliveryDate"].ToString(),
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

using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Application.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class OrderRepository : IOrderRepository
    {

        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            const string query = @"
                INSERT INTO Orders (UserId, DirectionId, OrderPaymentMethod, Status, OrderDeliveryDate, OrderCost, Weight, DeliveryFee)
                OUTPUT INSERTED.OrderId
                VALUES (@UserId, @DirectionId, @PaymentMethod, @Status, @DeliveryDate, 0, 0, 0);";
            var parameters = new List<SqlParameter>
            {
                new("@UserId", order.UserId),
                new("@DirectionId", order.DirectionId),
                new("@PaymentMethod", order.PaymentMethod),
                new("@DeliveryDate", order.DeliveryDate),
                new("@Status", order.Status)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var orderId = await command.ExecuteScalarAsync();
            return orderId.ToString();
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            var query = @"
            SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                   u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                   d.Coordinates, o.Status, o.OrderPaymentMethod, o.OrderDeliveryDate, o.DirectionId
            FROM Orders o
            JOIN Users u ON o.UserId = u.Id
            JOIN Directions d ON o.DirectionId = d.Id
            WHERE o.OrderId = @OrderId;";
            Order order = null;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                command.Parameters.AddWithValue("@OrderId", orderId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        order = new Order
                        {
                            OrderId = reader["OrderId"].ToString(),
                            UserId = reader["UserId"].ToString(),
                            DirectionId = reader["DirectionId"].ToString(),
                            PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                            OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                            DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                            OrderCost = Convert.ToDouble(reader["OrderCost"]),
                            Weight = Convert.ToDouble(reader["Weight"]),
                            DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                            Status = Convert.ToInt32(reader["Status"])
                        };
                    }
                }
            }
            return order;
        }

        public async Task<List<OrderDetails>> GetOrdersDetailsAsync()
        {
            var orders = new List<OrderDetails>();
            const string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var order = new OrderDetails
                {
                    Order = new Order
                    {
                        OrderId = reader["OrderId"].ToString(),
                        UserId = reader["UserId"].ToString(),
                        DirectionId = reader["DirectionId"].ToString(),
                        PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                        OrderCost = Convert.ToDouble(reader["OrderCost"]),
                        Weight = Convert.ToDouble(reader["Weight"]),
                        DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                        Status = Convert.ToInt32(reader["Status"])
                    },
                    UserName = reader["UserName"].ToString(),
                    Direction = reader["NumDirection"].ToString(),
                    UserEmail = reader["UserEmail"].ToString(),
                    OtherSigns = reader["OtherSigns"] as string,
                    Coordinates = reader["Coordinates"].ToString(),
                    Products = await GetProductsByOrderIdAsync(reader["OrderId"].ToString())
                };
                orders.Add(order);
            }
            return orders;
        }

        public async Task<OrderDetails> GetOrderDetailsByIdAsync(string orderId)
        {
            var query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.OrderId = @OrderId;";
            OrderDetails order = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                command.Parameters.AddWithValue("@OrderId", orderId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        order = new OrderDetails
                        {
                            Order = new Order
                            {
                                OrderId = reader["OrderId"].ToString(),
                                UserId = reader["UserId"].ToString(),
                                DirectionId = reader["DirectionId"].ToString(),
                                PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                                OrderCost = Convert.ToDouble(reader["OrderCost"]),
                                Weight = Convert.ToDouble(reader["Weight"]),
                                DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                                Status = Convert.ToInt32(reader["Status"])
                            },
                            UserName = reader["UserName"].ToString(),
                            Direction = reader["NumDirection"].ToString(),
                            UserEmail = reader["UserEmail"].ToString(),
                            OtherSigns = reader["OtherSigns"] as string,
                            Coordinates = reader["Coordinates"].ToString(),
                            Products = await GetProductsByOrderIdAsync(reader["OrderId"].ToString())
                        };
                    }
                }
            }
            return order;
        }

        public async Task<List<OrderDetails>> GetToConfirmOrdersAsync()
        {
            var query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.Status = 0;";
            var orders = new List<OrderDetails>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var order = new OrderDetails
                        {
                            Order = new Order
                            {
                                OrderId = reader["OrderId"].ToString(),
                                UserId = reader["UserId"].ToString(),
                                DirectionId = reader["DirectionId"].ToString(),
                                PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                                OrderCost = Convert.ToDouble(reader["OrderCost"]),
                                Weight = Convert.ToDouble(reader["Weight"]),
                                DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                                Status = Convert.ToInt32(reader["Status"])
                            },
                            UserName = reader["UserName"].ToString(),
                            Direction = reader["NumDirection"].ToString(),
                            UserEmail = reader["UserEmail"].ToString(),
                            OtherSigns = reader["OtherSigns"] as string,
                            Coordinates = reader["Coordinates"].ToString(),
                            Products = await GetProductsByOrderIdAsync(reader["OrderId"].ToString())
                        };
                        orders.Add(order);
                    }
                }
            }
            return orders;
        }

        public async Task<List<OrderDetails>> GetToConfirmOrdersByUserIdAsync(string userId)
        {
            var orders = new List<OrderDetails>();
            const string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.Status = 0 AND o.UserId = @UserId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var order = new OrderDetails
                {
                    Order = new Order
                    {
                        OrderId = reader["OrderId"].ToString(),
                        UserId = reader["UserId"].ToString(),
                        DirectionId = reader["DirectionId"].ToString(),
                        PaymentMethod = reader["OrderPaymentMethod"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        DeliveryDate = reader["OrderDeliveryDate"].ToString(),
                        OrderCost = Convert.ToDouble(reader["OrderCost"]),
                        Weight = Convert.ToDouble(reader["Weight"]),
                        DeliveryFee = Convert.ToDouble(reader["DeliveryFee"]),
                        Status = Convert.ToInt32(reader["Status"])
                    },
                    UserName = reader["UserName"].ToString(),
                    Direction = reader["NumDirection"].ToString(),
                    UserEmail = reader["UserEmail"].ToString(),
                    OtherSigns = reader["OtherSigns"] as string,
                    Coordinates = reader["Coordinates"].ToString(),
                    Products = await GetProductsByOrderIdAsync(reader["OrderId"].ToString())
                };
                orders.Add(order);
            }
            return orders;
        }

        public async Task<bool> ConfirmOrderAsync(string orderId)
        {
            var query = "UPDATE Orders SET Status = 1 WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DenyOrderAsync(string orderId)
        {
            var query = "UPDATE Orders SET Status = 5 WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AddProductToOrderAsync(AddProductToOrderRequest orderProduct, double totalWeight, double totalCost)
        {
            var query = @"
                INSERT INTO Order_Product (OrderId, ProductId, Amount, ProductsCost)
                VALUES (@OrderId, @ProductId, @Amount, @ProductsCost);";
            var parameters = new List<SqlParameter>
                {
                new("@OrderId", orderProduct.OrderId),
                new("@ProductId", orderProduct.ProductId),
                new("@Amount", orderProduct.Amount),
                new("@ProductsCost", totalCost)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            var updateQuery = @"
                UPDATE Orders
                SET OrderCost = OrderCost + @ProductsCost, Weight = Weight + @TotalWeight
                WHERE OrderId = @OrderId;";
            var updateParams = new List<SqlParameter>
                {
                new("@ProductsCost", totalCost),
                new("@TotalWeight", totalWeight),
                new("@OrderId", orderProduct.OrderId)
            };
            using var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddRange(updateParams.ToArray());
            return await updateCommand.ExecuteNonQueryAsync() > 0;

        }

        public async Task<bool> UpdateDeliveryFeeAsync(string orderId, double deliveryFee)
        {
            var query = "UPDATE Orders SET DeliveryFee = DeliveryFee + @DeliveryFee WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter>
            {
                new("@DeliveryFee", deliveryFee),
                new("@OrderId", orderId)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<ProductDetails>> GetProductsByOrderIdAsync(string orderId)
        {
            var query = @"
                SELECT op.ProductId, p.Name AS ProductName, op.Amount, op.ProductsCost, 
                       e.Name AS EnterpriseName, e.Email AS EnterpriseEmail
                FROM Order_Product op
                JOIN Products p ON op.ProductId = p.Id
                JOIN Enterprises e ON p.EnterpriseId = e.Id
                WHERE op.OrderId = @OrderId;";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            var products = new List<ProductDetails>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductDetails
                {
                    ProductId = reader["ProductId"].ToString(),
                    ProductName = reader["ProductName"].ToString(),
                    Quantity = Convert.ToInt32(reader["Amount"]),
                    ProductsCost = Convert.ToDouble(reader["ProductsCost"]),
                    EnterpriseName = reader["EnterpriseName"].ToString(),
                    EnterpriseEmail = reader["EnterpriseEmail"].ToString(),
                });
            }
            return products;
        }

        public async Task<double> GetProductEarningsByMonth(string productId, int month)
        {
            var query = @"
                SELECT SUM(op.ProductsCost) AS Earnings
                FROM Order_Product op
                JOIN Orders o ON op.OrderId = o.OrderId
                WHERE op.ProductId = @ProductId AND MONTH(o.OrderDate) = @Month AND o.Status = 4;";
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", productId),
                new("@Month", month)
            };
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            var earnings = await command.ExecuteScalarAsync();
            return earnings is DBNull ? 0 : Convert.ToDouble(earnings);
        }
    }
}












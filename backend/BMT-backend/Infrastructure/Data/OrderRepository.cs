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
        private readonly IProductRepository _productRepository;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
            _productRepository = new ProductRepository(connectionString);
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
            while (reader.Read()) {
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

        /*
         * 0 No confirmado
         * 1 Confirmado
         * 2 Listo para envío
         * 3 Shipping
         */
        public async Task<List<OrderDetails>> GetInProgressOrderAsync(string userId)
        {
            //Ocupo que me regrese los pedidos que estén en estado 0, 1, 2 o 3
            var query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.Status IN (0, 1, 2, 3) AND o.UserId = @UserId;";
            var orders = new List<OrderDetails>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
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
            var query = "UPDATE Orders SET DeliveryFee = @DeliveryFee WHERE OrderId = @OrderId";
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

        public async Task<bool> IsDirectionUsedInOrdersAsync(string directionId)
        {
            var query = "SELECT COUNT(*) FROM Orders WHERE DirectionId = @DirectionId";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DirectionId", directionId);
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<List<Product>> GetOrderProductsAsync(string userId)
        {
            var query = @"
                SELECT p.Id
                FROM Order_Product op
                JOIN Orders o ON op.OrderId = o.OrderId
                JOIN Products p ON op.ProductId = p.Id
                WHERE o.UserId = @UserId;";
            var parameters = new List<SqlParameter> { new("@UserId", userId) };
            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                var product = await _productRepository.GetProductDetailsByIdAsync(reader["Id"].ToString());
                if (!products.Exists(p => p.Id == product.Id)){
                    products.Add(product);
                }
            }
            return products;
        }

        public async Task<bool> IsProductUsedInOrdersAsync(string productId)
        {
            var query = "SELECT COUNT(*) FROM Order_Product WHERE ProductId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> AreEnterpriseProductsInOrders(string enterpriseId)
        {
            var query = @"
            SELECT COUNT(*)
            FROM Products p
            JOIN Order_Product op ON p.Id = op.ProductId
            WHERE p.EnterpriseId = @EnterpriseId";
        public async Task<List<OrderDetails>> GetOrderReportsByUserIdAsync(ReportRequest report)
        {
            var query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.UserId = @UserId AND o.OrderDate BETWEEN @FechaInicio AND @FechaFin AND o.Status BETWEEN @statusInicial AND @statusFinal;";
            var parameters = new List<SqlParameter>
            {
                new("@UserId", report.UserId),
                new("@FechaInicio", report.FechaInicio),
                new("@FechaFin", report.FechaFin),
                new("@statusInicial", report.statusInicial),
                new("@statusFinal", report.statusFinal)
            };
            var orders = new List<OrderDetails>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
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
            return orders;
        }


        public async Task<List<OrderDetails>> GetOrderReportsByEnterpriseIdAsync(ReportRequest report)
        {
            var query = @"
                SELECT o.OrderId
                FROM Orders o
                JOIN Order_Product op ON o.OrderId = op.OrderId
                JOIN Products p ON op.ProductId = p.Id
                WHERE p.EnterpriseId = @EnterpriseId AND o.OrderDate BETWEEN @FechaInicio AND @FechaFin AND o.Status BETWEEN @statusInicial AND @statusFinal;";
            var parameters = new List<SqlParameter>
            {
                new("@EnterpriseId", report.EnterpriseId),
                new("@FechaInicio", report.FechaInicio),
                new("@FechaFin", report.FechaFin),
                new("@statusInicial", report.statusInicial),
                new("@statusFinal", report.statusFinal)
            };
            var orders = new List<OrderDetails>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var order = await GetOrderDetailsByIdAsync(reader["OrderId"].ToString());
                if (!orders.Exists(o => o.Order.OrderId == order.Order.OrderId))
                {
                    orders.Add(order);
                }
            }
            return orders;
        }

        public async Task<List<OrderDetails>> GetOrderReportsAsync(ReportRequest report)
        {
            var query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.OrderDate BETWEEN @FechaInicio AND @FechaFin AND o.Status BETWEEN @statusInicial AND @statusFinal;";
            var parameters = new List<SqlParameter>
            {
                new("@FechaInicio", report.FechaInicio),
                new("@FechaFin", report.FechaFin),
                new("@statusInicial", report.statusInicial),
                new("@statusFinal", report.statusFinal)
            };
            var orders = new List<OrderDetails>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
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
            return orders;
        }
    }
}

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnterpriseId", enterpriseId);
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0; // Devuelve true si hay productos asociados a órdenes
                }
            }
        }
    }
}

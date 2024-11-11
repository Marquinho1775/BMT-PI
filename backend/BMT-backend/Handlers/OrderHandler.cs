using System;
using System.Data;
using System.Data.SqlClient;
using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;
using BMT_backend.Domain.Requests;

namespace BMT_backend.Handlers
{
    public class OrderHandler
    {
        private readonly string _connectionString;

        public OrderHandler(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BMTContext") + ";MultipleActiveResultSets=True";
        }

        private DataTable ExecuteQuery(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            using var adapter = new SqlDataAdapter(command);
            var resultTable = new DataTable();
            adapter.Fill(resultTable);
            return resultTable;
        }

        private int ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            connection.Open();
            return command.ExecuteNonQuery();
        }

        private object ExecuteScalar(string query, List<SqlParameter> parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());
            connection.Open();
            return command.ExecuteScalar();
        }

        public string CreateOrder(Order order)
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
            return ExecuteScalar(query, parameters)?.ToString();
        }

        public bool AddProductToOrder(AddProductToOrderRequest orderProduct)
        {
            const string insertQuery = @"
                INSERT INTO Order_Product (OrderId, ProductId, Amount, ProductsCost)
                VALUES (@OrderId, @ProductId, @Amount, @ProductsCost);";
            var insertParams = new List<SqlParameter>
            {
                new("@OrderId", orderProduct.OrderId),
                new("@ProductId", orderProduct.ProductId),
                new("@Amount", orderProduct.Amount),
                new("@ProductsCost", orderProduct.ProductsCost)
            };
            ExecuteNonQuery(insertQuery, insertParams);
            double productWeight = GetProductWeight(orderProduct.ProductId);
            double totalWeight = orderProduct.Amount * productWeight;
            const string updateQuery = @"
                UPDATE Orders
                SET OrderCost = OrderCost + @ProductsCost, Weight = Weight + @TotalWeight
                WHERE OrderId = @OrderId;";
            var updateParams = new List<SqlParameter>
            {
                new("@ProductsCost", orderProduct.ProductsCost),
                new("@TotalWeight", totalWeight),
                new("@OrderId", orderProduct.OrderId)
            };
            return ExecuteNonQuery(updateQuery, updateParams) > 0;
        }

        private double GetProductWeight(string productId)
        {
            const string query = "SELECT Weight FROM Products WHERE Id = @ProductId";
            var parameters = new List<SqlParameter> { new("@ProductId", productId) };
            return Convert.ToDouble(ExecuteScalar(query, parameters));
        }

        public bool UpdateDeliveryFee(string orderId)
        {
            var orderDetails = GetOrderDetails(orderId);
            double deliveryFee = CalculateDeliveryFee(orderDetails.DirectionId, orderDetails.Weight);
            const string query = "UPDATE Orders SET DeliveryFee = @DeliveryFee WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter>
            {
                new("@DeliveryFee", deliveryFee),
                new("@OrderId", orderId)
            };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        private (string DirectionId, double Weight) GetOrderDetails(string orderId)
        {
            const string query = "SELECT DirectionId, Weight FROM Orders WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            var resultTable = ExecuteQuery(query, parameters);
            var row = resultTable.Rows[0];
            return (row["DirectionId"].ToString(), Convert.ToDouble(row["Weight"]));
        }

        private double CalculateDeliveryFee(string directionId, double weight)
        {
            var coordinates = GetCoordinates(directionId);
            var coord = coordinates.Split(',');
            double x = Convert.ToDouble(coord[0]);
            double y = Convert.ToDouble(coord[1]);
            double distance = DistanceCalculator.CalcularDistancia(x, y);
            double deliveryFee = distance >= 25 ? 3000 : 2000;
            deliveryFee += weight * 200;
            return deliveryFee;
        }

        private string GetCoordinates(string directionId)
        {
            const string query = "SELECT Coordinates FROM Directions WHERE Id = @DirectionId";
            var parameters = new List<SqlParameter> { new("@DirectionId", directionId) };
            return ExecuteScalar(query, parameters)?.ToString();
        }

        public List<Order> GetOrders()
        {
            const string query = "SELECT * FROM Orders";
            var resultTable = ExecuteQuery(query);
            var orders = new List<Order>();
            foreach (DataRow row in resultTable.Rows)
            {
                orders.Add(CreateOrderFromRow(row));
            }
            return orders;
        }

        public List<OrderDetails> GetToConfirmOrders()
        {
            const string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, d.Id AS DirectionId, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.Status = 0;";
            var resultTable = ExecuteQuery(query);
            var orders = new List<OrderDetails>();
            foreach (DataRow row in resultTable.Rows)
            {
                var order = CreateOrderDetailFromRow(row);
                orders.Add(order);
            }
            return orders;
        }

        public List<OrderDetails> GetToConfirmUserOrders(string userId)
        {
            const string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.Status = 0 AND o.UserId = @UserId;";
            var parameters = new List<SqlParameter> { new("@UserId", userId) };
            var resultTable = ExecuteQuery(query, parameters);
            var orders = new List<OrderDetails>();
            foreach (DataRow row in resultTable.Rows)
            {
                var order = CreateOrderDetailFromRow(row);
                orders.Add(order);
            }
            return orders;
        }

        public OrderDetails GetOrderById(string orderId)
        {
            const string query = @"
                SELECT o.OrderId, o.OrderDate, o.OrderCost, o.DeliveryFee, o.Weight, o.UserId,
                       u.UserName, d.NumDirection, d.OtherSigns, u.Email AS UserEmail, 
                       d.Coordinates, o.Status, o.OrderPaymentMethod, o.OrderDeliveryDate
                FROM Orders o
                JOIN Users u ON o.UserId = u.Id
                JOIN Directions d ON o.DirectionId = d.Id
                WHERE o.OrderId = @OrderId;";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            var resultTable = ExecuteQuery(query, parameters);
            var order = CreateOrderDetailFromRow(resultTable.Rows[0]);
            return order;
        }

        private List<ProductDetails> GetProductsByOrderId(string orderId)
        {
            const string query = @"
                SELECT op.ProductId, p.Name AS ProductName, op.Amount, op.ProductsCost, 
                       e.Name AS EnterpriseName, e.Email AS EnterpriseEmail
                FROM Order_Product op
                JOIN Products p ON op.ProductId = p.Id
                JOIN Enterprises e ON p.EnterpriseId = e.Id
                WHERE op.OrderId = @OrderId;";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            var resultTable = ExecuteQuery(query, parameters);
            var products = new List<ProductDetails>();
            foreach (DataRow row in resultTable.Rows)
            {
                products.Add(new ProductDetails
                {
                    ProductId = row["ProductId"].ToString(),
                    ProductName = row["ProductName"].ToString(),
                    Quantity = Convert.ToInt32(row["Amount"]),
                    ProductsCost = Convert.ToInt32(row["ProductsCost"]),
                    EnterpriseName = row["EnterpriseName"].ToString(),
                    EnterpriseEmail = row["EnterpriseEmail"].ToString(),
                });
            }
            return products;
        }

        public bool ConfirmOrder(string orderId)
        {
            const string query = "UPDATE Orders SET Status = 1 WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DenyOrder(string orderId)
        {
            const string query = "UPDATE Orders SET Status = 5 WHERE OrderId = @OrderId";
            var parameters = new List<SqlParameter> { new("@OrderId", orderId) };
            return ExecuteNonQuery(query, parameters) > 0;
        }

        private Order CreateOrderFromRow(DataRow row)
        {
            return new Order
            {
                OrderId = row["OrderId"].ToString(),
                UserId = row["UserId"].ToString(),
                DirectionId = row["DirectionId"].ToString(),
                PaymentMethod = row["OrderPaymentMethod"].ToString(),
                OrderDate = Convert.ToDateTime(row["OrderDate"]),
                DeliveryDate = row["OrderDeliveryDate"].ToString(),
                OrderCost = Convert.ToDouble(row["OrderCost"]),
                Weight = Convert.ToDouble(row["Weight"]),
                DeliveryFee = Convert.ToDouble(row["DeliveryFee"]),
                Status = Convert.ToInt32(row["Status"])
            };
        }

        private OrderDetails CreateOrderDetailFromRow(DataRow row)
        {
            var orderId = row["OrderId"].ToString();
            var order = new OrderDetails
            {
                Order = new Order
                {
                    OrderId = orderId,
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    OrderCost = Convert.ToDouble(row["OrderCost"]),
                    DeliveryFee = Convert.ToDouble(row["DeliveryFee"]),
                    Weight = Convert.ToDouble(row["Weight"]),
                    UserId = row["UserId"].ToString(),
                    Status = Convert.ToInt32(row["Status"]),
                    DirectionId = row.Table.Columns.Contains("DirectionId") ? row["DirectionId"].ToString() : null,
                    PaymentMethod = row.Table.Columns.Contains("OrderPaymentMethod") ? row["OrderPaymentMethod"].ToString() : null,
                    DeliveryDate = row.Table.Columns.Contains("OrderDeliveryDate") ? row["OrderDeliveryDate"].ToString() : null
                },
                UserName = row["UserName"].ToString(),
                Direction = row["NumDirection"].ToString(),
                OtherSigns = row["OtherSigns"].ToString(),
                UserEmail = row["UserEmail"].ToString(),
                Coordinates = row["Coordinates"].ToString(),
                Products = GetProductsByOrderId(orderId)
            };
            return order;
        }
    }
}
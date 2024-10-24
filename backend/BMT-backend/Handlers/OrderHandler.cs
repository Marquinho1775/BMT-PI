using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using BMT_backend.Models;

namespace BMT_backend.Handlers
{
    public class OrderHandler
    {
        private readonly SqlConnection _connection;

        public OrderHandler(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("BMTContext"));
        }

        private DataTable CreateQueryTable(string query)
        {
            using var queryCommand = new SqlCommand(query, _connection);
            using var tableAdapter = new SqlDataAdapter(queryCommand);
            var table = new DataTable();
            _connection.Open();
            tableAdapter.Fill(table);
            _connection.Close();
            return table;
        }

        public string CreateOrder(ShoppingCart cart, string userId, string userName, DirectionModel address)
        {
            var order = new OrderModel
            {
                OrderDate = DateTime.Now,
                OrderCost = cart.amount,
                DeliveryFee = 1000,
                Weight = CalculateTotalWeight(cart),
                UserId = userId,
                UserName = userName,
                DirectionName = address. NumDirection,
                Province = address.Province,
                Canton = address.Canton,
                District = address.District,
                OtherSigns = address.OtherSigns,
                Coordinates = address.Coordinates,
                Status = 0, // Estado inicial del pedido 0, 1, 2, 3, 4 o 5
                Products = ConvertCartToProducts(cart)
            };

            return InsertOrderToDatabase(order);
        }

        private decimal CalculateTotalWeight(ShoppingCart cart)
        {
            decimal totalWeight = 0;
            foreach (var item in cart.Items)
            {
                totalWeight += item.Quantity * GetProductWeight(item.ProductId);
            }
            return totalWeight;
        }

        private int GetProductWeight(object productId)
        {
            throw new NotImplementedException();
        }

        private List<ProductDetails> ConvertCartToProducts(ShoppingCart cart, EnterpriseModel enterprise)
        {
            var products = new List<ProductDetails>();
            foreach (var item in cart.Items)
            {
                products.Add(new ProductDetails
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    EnterpriseName = item.EnterpriseId
                });
            }
            return products;
        }



        private string InsertOrderToDatabase(OrderModel order)
        {
            var query = @"
                INSERT INTO Orders (OrderDate, OrderCost, DeliveryFee, Weight, UserId, UserName, 
                DirectionName, Province, Canton, District, OtherSigns, Coordinates, Status, Products) 
                OUTPUT INSERTED.OrderId
                VALUES (@OrderDate, @OrderCost, @DeliveryFee, @Weight, @UserId, @UserName, 
                @DirectionName, @Province, @Canton, @District, @OtherSigns, @Coordinates, 
                @Status, @Products);";

            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@OrderCost", order.OrderCost);
            command.Parameters.AddWithValue("@DeliveryFee", order.DeliveryFee);
            command.Parameters.AddWithValue("@Weight", order.Weight);
            command.Parameters.AddWithValue("@UserId", order.UserId);
            command.Parameters.AddWithValue("@UserName", order.UserName);
            command.Parameters.AddWithValue("@DirectionName", order.DirectionName);
            command.Parameters.AddWithValue("@Province", order.Province);
            command.Parameters.AddWithValue("@Canton", order.Canton);
            command.Parameters.AddWithValue("@District", order.District);
            command.Parameters.AddWithValue("@OtherSigns", (object)order.OtherSigns ?? DBNull.Value);
            command.Parameters.AddWithValue("@Coordinates", order.Coordinates);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@Products", JsonConvert.SerializeObject(order.Products));

            _connection.Open();
            var orderId = command.ExecuteScalar()?.ToString();
            _connection.Close();

            return orderId ?? string.Empty;
        }

        public List<OrderModel> GetOrders()
        {
            var orders = new List<OrderModel>();
            string query = "SELECT * FROM Orders;";

            using var command = new SqlCommand(query, _connection);
            var table = CreateQueryTable(query);

            foreach (DataRow row in table.Rows)
            {
                orders.Add(new OrderModel
                {
                    OrderId = row["OrderId"].ToString(),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    OrderCost = Convert.ToDecimal(row["OrderCost"]),
                    DeliveryFee = Convert.ToDecimal(row["DeliveryFee"]),
                    Weight = Convert.ToDecimal(row["Weight"]),
                    UserId = row["UserId"].ToString(),
                    UserName = row["UserName"].ToString(),
                    DirectionName = row["DirectionNum"].ToString(),
                    Status = Convert.ToInt32(row["Status"]),
                    Products = JsonConvert.DeserializeObject<List<ProductDetails>>(row["Products"].ToString())
                });
            }
            return orders;
        }

        public OrderModel GetOrderById(int orderId)
        {
            string query = "SELECT * FROM Orders WHERE OrderId = @OrderId;";

            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            _connection.Open();
            using var reader = command.ExecuteReader();
            OrderModel order = null;

            if (reader.Read())
            {
                order = new OrderModel
                {
                    OrderId = (string)reader["OrderId"],
                    OrderDate = (DateTime)reader["OrderDate"],
                    OrderCost = (decimal)reader["OrderCost"],
                    DeliveryFee = (decimal)reader["DeliveryFee"],
                    Weight = (decimal)reader["Weight"],
                    UserId = reader["UserId"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    DirectionName = reader["DirectionName"].ToString(),
                    Province = reader["Province"].ToString(),
                    Canton = reader["Canton"].ToString(),
                    District = reader["District"].ToString(),
                    OtherSigns = reader["OtherSigns"] as string,
                    Coordinates = reader["Coordinates"].ToString(),
                    Status = (int)reader["Status"],
                    Products = JsonConvert.DeserializeObject<List<ProductDetails>>(reader["Products"].ToString())
                };
            }

            _connection.Close();
            return order; // Retorna null si el order no se encuentra
        }

        public bool UpdateOrderStatus(string orderId, int newStatus)
        {
            string updateQuery = "UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId;";

            using var command = new SqlCommand(updateQuery, _connection);
            command.Parameters.AddWithValue("@Status", newStatus);
            command.Parameters.AddWithValue("@OrderId", orderId);

            _connection.Open();
            var result = command.ExecuteNonQuery() >= 1;
            _connection.Close();

            return result;
        }
    }
}
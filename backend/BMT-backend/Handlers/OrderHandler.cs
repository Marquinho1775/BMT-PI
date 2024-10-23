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
            SqlCommand queryCommand = new SqlCommand(query, _connection);
            SqlDataAdapter tableAdapter = new SqlDataAdapter(queryCommand);
            DataTable tableFormatQuery = new DataTable();
            _connection.Open();
            tableAdapter.Fill(tableFormatQuery);
            _connection.Close();
            return tableFormatQuery;
        }

        public string CreateOrder(OrderModel order)
        {
            string createOrderQuery = @"
                INSERT INTO Orders (OrderDate, OrderCost, DeliveryFee, Weight, UserId, UserName, DirectionNum, Status, Products) 
                OUTPUT INSERTED.OrderId
                VALUES (@OrderDate, @OrderCost, @DeliveryFee, @Weight, @UserId, @UserName, @DirectionNum, @Status, @Products);";

            using var command = new SqlCommand(createOrderQuery, _connection);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@OrderCost", order.OrderCost);
            command.Parameters.AddWithValue("@DeliveryFee", order.DeliveryFee);
            command.Parameters.AddWithValue("@Weight", order.Weight);
            command.Parameters.AddWithValue("@UserId", order.UserId);
            command.Parameters.AddWithValue("@UserName", order.UserName);
            command.Parameters.AddWithValue("@DirectionNum", order.DirectionNum);
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
                    DirectionNum = row["DirectionNum"].ToString(),
                    Status = Convert.ToInt32(row["Status"]),
                    Products = JsonConvert.DeserializeObject<List<ProductDetails>>(row["Products"].ToString())
                });
            }
            return orders;
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
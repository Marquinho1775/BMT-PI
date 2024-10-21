using BMT_backend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
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

        // Method to create a new order (assuming DeliveryAddress is not required)
        public bool CreateOrder(Order order)
        {
            var query = "INSERT INTO dbo.Orders (DeliveryDate) VALUES (@DeliveryDate)";
            var queryCommand = new SqlCommand(query, Connection);
            queryCommand.Parameters.AddWithValue("@DeliveryDate", order.DeliveryDate);

            Connection.Open();
            bool result = queryCommand.ExecuteNonQuery() >= 1;
            Connection.Close();

            // If successful, retrieve the newly created order ID and update the OrderItems
            if (result)
            {
                // Logic to retrieve the newly created order ID (e.g., using SCOPE_IDENTITY())
                //order.Id = ;
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = order.Id;
                    // Call a separate method to create the OrderItem (not shown here)
                    CreateOrderItem(item);
                }
            }

            return result;
        }

        internal object GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        // Method to create a new order item (assuming OrderId is already set)
        private void CreateOrderItem(OrderItem orderItem)
        {
            var query = "INSERT INTO dbo.OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)";
            var queryCommand = new SqlCommand(query, Connection);
            queryCommand.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
            queryCommand.Parameters.AddWithValue("@ProductId", orderItem.ProductId);
            queryCommand.Parameters.AddWithValue("@Quantity", orderItem.Quantity);

            Connection.Open();
            queryCommand.ExecuteNonQuery();
            Connection.Close();
        }
    }
}

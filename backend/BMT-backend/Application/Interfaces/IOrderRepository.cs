using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<string> CreateOrderAsync(Order order);

        Task<Order> GetOrderByIdAsync(string orderId);
        Task<List<OrderDetails>> GetOrdersDetailsAsync();
        Task<OrderDetails> GetOrderDetailsByIdAsync(string orderId);

        Task<List<OrderDetails>> GetToConfirmOrdersAsync();
        Task<List<OrderDetails>> GetToConfirmOrdersByUserIdAsync(string userId);

        Task<bool> ConfirmOrderAsync(string orderId);
        Task<bool> DenyOrderAsync(string orderId);
        
        Task<bool> AddProductToOrderAsync(AddProductToOrderRequest orderProduct, double Weight, double totalCost);
        Task<bool> UpdateDeliveryFeeAsync(string orderId, double deliveryFee);
        
        Task<List<ProductDetails>> GetProductsByOrderIdAsync(string orderId);
        Task<double> GetProductEarningsByMonth(string productId, int month);

    }
}

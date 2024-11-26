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
        Task<List<OrderDetails>> GetInProgressOrderAsync(string userId);

        Task<bool> ConfirmOrderAsync(string orderId);
        Task<bool> DenyOrderAsync(string orderId, int roleId);
        
        Task<bool> AddProductToOrderAsync(AddProductToOrderRequest orderProduct, double Weight, double totalCost);
        Task<bool> UpdateDeliveryFeeAsync(string orderId, double deliveryFee);
        
        Task<List<ProductDetails>> GetProductsByOrderIdAsync(string orderId);
        Task<double> GetProductEarningsByMonth(string productId, int month);
        Task<double> GetProductEarningsByDay(string productId, DateTime date);

        Task<bool> IsDirectionUsedInOrdersAsync(string directionId);
        Task<bool> IsProductUsedInOrdersAsync(string productId);
        Task<bool> AreEnterpriseProductsInOrders(string enterpriseId);
        Task<List<OrderDetails>> GetOrderReportsByUserIdAsync(ReportRequest report);
        Task<List<OrderDetails>> GetOrderReportsByEnterpriseIdAsync(ReportRequest report);
        Task<List<OrderDetails>> GetOrderReportsAsync(ReportRequest report);
        Task<List<Product>> GetOrderProductsAsync(string userId);

    }
}

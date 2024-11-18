using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ProductService _productService;
        private readonly DirectionService _directionService;

        public OrderService(IOrderRepository orderRepository, ProductService productService, DirectionService directionService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _directionService = directionService;
        }

        public async Task<string> CreateOrder(Order order)
        {
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<List<OrderDetails>> GetOrdersDetails()
        {
            return await _orderRepository.GetOrdersDetailsAsync();
        }

        public async Task<OrderDetails> GetOrderDetailsById(string orderId)
        {
            return await _orderRepository.GetOrderDetailsByIdAsync(orderId);
        }

        public async Task<List<OrderDetails>> GetToConfirmOrders()
        {
            return await _orderRepository.GetToConfirmOrdersAsync();
        }

        public async Task<List<OrderDetails>> GetToConfirmOrdersByUserId(string userId)
        {
            return await _orderRepository.GetToConfirmOrdersByUserIdAsync(userId);
        }

        public async Task<bool> ConfirmOrder(string orderId)
        {
            return await _orderRepository.ConfirmOrderAsync(orderId);
        }

        public async Task<bool> DenyOrder(string orderId)
        {
            return await _orderRepository.DenyOrderAsync(orderId);
        }   

        public async Task<bool> AddProductToOrder(AddProductToOrderRequest orderProduct)
        {
            Product product = await _productService.GetProductDetailsByIdAsync(orderProduct.ProductId);
            Order order = await _orderRepository.GetOrderByIdAsync(orderProduct.OrderId);
            double totalWeight = orderProduct.Amount * product.Weight;
            double totalCost = product.Price * orderProduct.Amount;
            await _orderRepository.AddProductToOrderAsync(orderProduct, totalWeight, totalCost);
            await _productService.UpdateStockAsync(product.Id, orderProduct.Amount, product.Type, orderProduct.Date);
            double deliveryFee = await CalculateDeliveryFeeAsync(order.DirectionId, totalWeight);
            await _orderRepository.UpdateDeliveryFeeAsync(orderProduct.OrderId, deliveryFee);
            return true;
        }

        private async Task<double> CalculateDeliveryFeeAsync(string directionId, double weight)
        {
            var coordinates = await _directionService.GetCoordinates(directionId);
            var coord = coordinates.Split(',');
            double x = Convert.ToDouble(coord[0]);
            double y = Convert.ToDouble(coord[1]);
            double distance = DistanceCalculator.CalcularDistancia(x, y);
            double deliveryFee = distance >= 25 ? 3000 : 2000;
            deliveryFee += weight * 200;
            return deliveryFee;
        }
    }
}

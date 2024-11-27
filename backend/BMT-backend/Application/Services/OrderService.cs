using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Presentation.DTOs;
using System.Collections.Generic;

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

        public async Task<List<OrderDetails>> GetInProgressOrder(string userId)
        {
            return await _orderRepository.GetInProgressOrderAsync(userId);
        }

        public async Task<bool> ConfirmOrder(string orderId)
        {
            return await _orderRepository.ConfirmOrderAsync(orderId);
        }

        public async Task<bool> DenyOrder(string orderId, int roleId)
        {
            return await _orderRepository.DenyOrderAsync(orderId, roleId);
        }

        public async Task<List<Product>> GetOrderProductsAsync(string userId)
        {
            return await _orderRepository.GetOrderProductsAsync(userId);
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

        public async Task<List<ReportDto>> GetOrderReportsAsync(ReportRequest report)
        {
            this.ValidateReportData(report);

            List<OrderDetails> response = null;

            if (report.UserId != null)
            {
                response = await _orderRepository.GetOrderReportsByUserIdAsync(report);
            }
            else if (report.EnterpriseId != null)
            {

                response = await _orderRepository.GetOrderReportsByEnterpriseIdAsync(report);
            }
            else
            {
                response = await _orderRepository.GetOrderReportsAsync(report);
            }

            List<ReportDto> reportList = new List<ReportDto>();

            if (report.statusInicial == 0)
            {
                reportList = FormatPendingOrders(response);
            }
            else if (report.statusInicial == 4)
            {
                reportList = FormatCompletedOrders(response);
            }
            else
            {
                reportList = FormatCanceledOrders(response);
            }
            return reportList;
        }

        public async Task<List<OrderDetails>> GetOrdersByDateAndStatusAsync(ReportRequest report)
        {
            this.ValidateReportData(report);

            List<OrderDetails> response = null;

            if (report.UserId != null)
            {
                response = await _orderRepository.GetOrderReportsByUserIdAsync(report);
            }
            else if (report.EnterpriseId != null)
            {

                response = await _orderRepository.GetOrderReportsByEnterpriseIdAsync(report);
            }
            else
            {
                response = await _orderRepository.GetOrderReportsAsync(report);
            }
            return response;
        }

        private List<ReportDto> FormatPendingOrders(List<OrderDetails> orders)
        {
            List<ReportDto> reportList = new List<ReportDto>();

            foreach (var order in orders)
            {
                ReportDto report = new ReportDto();
                report.NumOrder = order.Order.NumOrder;
                report.Enterprises = getEnterprise(order);
                report.ItemsCount = order.Products.Count;
                report.DateOfCreation = (DateTime)order.Order.OrderDate;
                report.DateOfDelivery = DateTime.TryParse(order.Order.DeliveryDate, out DateTime deliveryDate) ? (DateTime?)deliveryDate : null;
                report.Status = StatusToString(order.Order.Status);
                report.ProductCost = (double)order.Order.OrderCost;
                report.FeeCost = (double)order.Order.DeliveryFee;
                report.TotalCost = (double)order.Order.OrderCost + report.FeeCost;
                reportList.Add(report);
            }
            return reportList;
        }

        private List<ReportDto> FormatCompletedOrders(List<OrderDetails> orders)
        {
            List<ReportDto> reportList = new List<ReportDto>();

            foreach (var order in orders)
            {
                ReportDto report = new ReportDto();
                report.NumOrder = order.Order.NumOrder ?? throw new InvalidOperationException("Order number cannot be null.");
                report.Enterprises = getEnterprise(order);
                report.ItemsCount = order.Products.Count;
                report.DateOfCreation = order.Order.OrderDate ?? throw new InvalidOperationException("Order date cannot be null.");
                report.DateOfDelivery = DateTime.TryParse(order.Order.DeliveryDate, out DateTime deliveryDate) ? (DateTime?)deliveryDate : null;
                report.DateReceived = DateTime.TryParse(order.Order.DeliveryDate, out DateTime dateReceived) ? (DateTime?)dateReceived : null;
                report.ProductCost = order.Order.OrderCost ?? throw new InvalidOperationException("Order cost cannot be null.");
                report.FeeCost = order.Order.DeliveryFee ?? throw new InvalidOperationException("Delivery fee cannot be null.");
                report.TotalCost = report.ProductCost + report.FeeCost;
                reportList.Add(report);
            }
            return reportList;
        }

        private List<ReportDto> FormatCanceledOrders(List<OrderDetails> orders)
        {
            List<ReportDto> reportList = new List<ReportDto>();

            foreach (var order in orders)
            {
                ReportDto report = new ReportDto();
                report.NumOrder = order.Order.NumOrder;
                report.Enterprises = getEnterprise(order);
                report.ItemsCount = order.Products.Count;
                report.DateOfCreation = (DateTime)order.Order.OrderDate;
                report.DateOfCancelation = DateTime.TryParse(order.Order.DeliveryDate, out DateTime deliveryDate) ? (DateTime?)deliveryDate : null;
                if (order.Order.Status == 5)
                {
                    report.CancelBy = "Cliente";
                }
                else
                {
                    report.CancelBy = "Administrador";
                }
                report.ProductCost = (double)order.Order.OrderCost;
                report.FeeCost = (double)order.Order.DeliveryFee;
                report.TotalCost = (double)order.Order.OrderCost + report.FeeCost;
                reportList.Add(report);
            }
            return reportList;
        }

        private string StatusToString(int status)
        {
            switch (status)
            {
                case 0:
                    return "No confirmado";
                case 1:
                    return "Confirmado";
                case 2:
                    return "Listo para envío";
                case 3:
                    return "Enviando";
                case 4:
                    return "Terminado";
                case 5:
                    return "Cancelado por usuario";
                case 6:
                    return "Cancelado por dev";
                default:
                    return "Invalido";
            }
        }

        private string getEnterprise(OrderDetails orden)
        {
            List<string> enterprises = new List<string>();
            foreach (var product in orden.Products)
            {
                if (!enterprises.Contains(product.EnterpriseName))
                {
                    enterprises.Add(product.EnterpriseName);
                }
            }
            return string.Join(", ", enterprises);
        }

        private void ValidateReportData(ReportRequest report)
        {
            if (report.FechaInicio == null)
            {
                throw new ArgumentException("La fecha de inicio es obligatoria.");
            }
            if (report.FechaFin == null)
            {
                throw new ArgumentException("La fecha de final es obligatoria.");
            }
            if (report.statusInicial == null)
            {
                throw new ArgumentException("El estado inicial es obligatorio.");
            }
            if (report.statusFinal == null)
            {
                throw new ArgumentException("El estado final es obligatorio.");
            }
            if (report.FechaInicio > report.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha final.");
            }
        }

        public async Task<bool> IsDirectionUsedInOrders(string directionId)
        {
            return await _orderRepository.IsDirectionUsedInOrdersAsync(directionId);
        }

        public async Task<bool> IsProductUsedInOrders(string productId)
        {
            return await _orderRepository.IsProductUsedInOrdersAsync(productId);
        }

        public async Task<bool> AreEnterpriseProductsInOrders(string enterpriseId)
        {
            return await _orderRepository.AreEnterpriseProductsInOrders(enterpriseId);
        }
    }
}

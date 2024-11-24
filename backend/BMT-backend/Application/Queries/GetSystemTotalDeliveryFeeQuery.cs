using BMT_backend.Application.Interfaces;
using BMT_backend.Presentation.Requests;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Queries
{
    public class GetSystemTotalDeliveryFeeQuery
    {
        private readonly IOrderRepository _orderRepository;

        public GetSystemTotalDeliveryFeeQuery(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<double>> GetSystemTotalDeliverysFee()
        {
            List<double> deliveryFeeData = new List<double>(new double[12]);
            int currentYear = DateTime.Now.Year;
            for (int i = 0; i < 12; i++)
            {
                double monthlyFee = 0;
                DateTime startDate = new DateTime(currentYear, i + 1, 1);
                DateTime endDate = new DateTime(currentYear, i + 1, DateTime.DaysInMonth(currentYear, i + 1));
                ReportRequest request = new ReportRequest
                {
                    FechaInicio = startDate,
                    FechaFin = endDate,
                    statusInicial = 2,
                    statusFinal = 4
                };
                List<OrderDetails> orders = await _orderRepository.GetOrderReportsAsync(request);
                foreach (var order in orders)
                {
                    if (order.Order.DeliveryFee.HasValue)
                    {
                        monthlyFee += order.Order.DeliveryFee.Value;
                    }
                }
                deliveryFeeData[i] = monthlyFee;
            }
            return deliveryFeeData;
        }
    }
}

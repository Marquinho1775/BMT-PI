using BMT_backend.Application.Interfaces;
using BMT_backend.Application.Services;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Domain.Entities;
using System.Globalization;

namespace BMT_backend.Application.Queries
{
    public class GetEnterpriseWeeklyEarningsQuery
    {
        private readonly IOrderRepository _orderRepository;
        private readonly EnterpriseService _enterpriseService;

        public GetEnterpriseWeeklyEarningsQuery(IOrderRepository orderRepository, EnterpriseService enterpriseService)
        {
            _orderRepository = orderRepository;
            _enterpriseService = enterpriseService;
        }

        public async Task<List<EarningsDatasetDto>> GetEnterpriseWeeklyEarningsAsync(string enterpriseId)
        {
            try
            {
                List<EarningsDatasetDto> earnings = new List<EarningsDatasetDto>();
                List<Product> products = await _enterpriseService.GetEnterpriseProducts(enterpriseId);
                foreach (var product in products)
                {
                    EarningsDatasetDto dailyEarnings = new EarningsDatasetDto
                    {
                        Label = product.Name,
                        EarningsPerMonth = new List<double>(new double[7])
                    };
                    for (int day = 0; day < 7; day++)
                    {
                        double productEarnings = await _orderRepository.GetProductEarningsByDay(product.Id, DateTime.Now.AddDays(-day));
                        dailyEarnings.EarningsPerMonth[day] = productEarnings;
                    };
                    earnings.Add(dailyEarnings);
                }
                return earnings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener las ganancias semanales de la empresa");
            }
        }

            public async Task<List<EarningsDatasetDto>> GetSystemWeeklyEarningsAsync()
            {
            try
            {
                List<EarningsDatasetDto> earnings = new List<EarningsDatasetDto>();
                List<Enterprise> enterprises = await _enterpriseService.GetAllEnterprisesAsync();
                foreach (Enterprise enterprise in enterprises)
                {
                    List<EarningsDatasetDto> enterpriseProductEarnings = await GetEnterpriseWeeklyEarningsAsync(enterprise.Id);
                    EarningsDatasetDto enterpriseEarnings = new()
                    {
                        Label = enterprise.Name,
                        EarningsPerMonth = new List<double>(new double[7])
                    };
                    for (int i = 0; i < 7; i++)
                    {
                        double EnterpriseMonthEarnings = 0;
                        foreach (var productsEarnings in enterpriseProductEarnings)
                        {
                            EnterpriseMonthEarnings += productsEarnings.EarningsPerMonth[i];
                        }
                        enterpriseEarnings.EarningsPerMonth[i] = EnterpriseMonthEarnings;
                    }
                    earnings.Add(enterpriseEarnings);
                }
                return earnings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener las ganancias semanales del sistema");
            }
        }
    }
}

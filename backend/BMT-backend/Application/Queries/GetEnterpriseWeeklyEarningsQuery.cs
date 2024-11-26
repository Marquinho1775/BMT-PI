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
                for (int day = 0; day < 7; day++)
                {
                    EarningsDatasetDto dailyEarnings = new EarningsDatasetDto
                    {
                        Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DateTime.Now.AddDays(-day).DayOfWeek),
                        EarningsPerMonth = new List<double> { 0 } // Solo un valor por día
                    };
                    foreach (var product in products)
                    {
                        double productEarnings = await _orderRepository.GetProductEarningsByDay(product.Id, DateTime.Now.AddDays(-day));
                        dailyEarnings.EarningsPerMonth[0] += productEarnings;
                    }
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
                for (int day = 0; day < 7; day++)
                {
                    EarningsDatasetDto dailyEarnings = new EarningsDatasetDto
                    {
                        Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(DateTime.Now.AddDays(-day).DayOfWeek),
                        EarningsPerMonth = new List<double> { 0 }
                    };
                    foreach (var enterprise in enterprises)
                    {
                        List<Product> products = await _enterpriseService.GetEnterpriseProducts(enterprise.Id);
                        foreach (var product in products)
                        {
                            double productEarnings = await _orderRepository.GetProductEarningsByDay(product.Id, DateTime.Now.AddDays(-day));
                            dailyEarnings.EarningsPerMonth[0] += productEarnings;
                        }
                    }
                    earnings.Add(dailyEarnings);
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

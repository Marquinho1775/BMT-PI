using BMT_backend.Application.Services;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Queries
{
    public class GetAllEnterprisesEarningsQuery
    {
        private readonly EnterpriseService _enterpriseService;
        private readonly GetEnterpriseEarningsQuery _getEnterpriseEarningsQuery;

        public GetAllEnterprisesEarningsQuery(EnterpriseService enterpriseService, GetEnterpriseEarningsQuery getEnterpriseEarningsQuery)
        {
            _enterpriseService = enterpriseService;
            _getEnterpriseEarningsQuery = getEnterpriseEarningsQuery;
        }

        public async Task<List<EarningsDatasetDto>> GetAllEnterprisesEarningsAsync()
        {
            try
            {
                List<EarningsDatasetDto> earnings = [];
                List<Enterprise> enterprises = await _enterpriseService.GetAllEnterprisesAsync();
                foreach (Enterprise enterprise in enterprises)
                {
                    List<EarningsDatasetDto> enterpriseProductEarnings = await _getEnterpriseEarningsQuery.GetEnterpriseEarningsAsync(enterprise.Id);
                    EarningsDatasetDto enterpriseEarnings = new()
                    {
                        Label = enterprise.Name,
                        EarningsPerMonth = new List<double>(new double[12])
                    };
                    for (int i = 0; i < 12; i++)
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
                throw new Exception("Error al obtener las ganancias de la empresa");
            }
        }
    }
}
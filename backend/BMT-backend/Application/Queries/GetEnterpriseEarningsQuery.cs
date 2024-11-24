using BMT_backend.Application.Interfaces;
using BMT_backend.Application.Services;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Domain.Entities;


namespace BMT_backend.Application.Queries
{
    public class GetEnterpriseEarningsQuery
    {
        private readonly IOrderRepository _orderRepository;
        private readonly EnterpriseService _enterpriseService;

        public GetEnterpriseEarningsQuery(IOrderRepository orderRepository, EnterpriseService enterpriseService)
        {
            _orderRepository = orderRepository;
            _enterpriseService = enterpriseService;
        }

        public async Task<List<ProductEarningsDataset>> GetEnterpriseEarningsAsync (string enterpriseId)
        {
            try
            {
                List<ProductEarningsDataset> earnings = [];
                List<Product> products = await _enterpriseService.GetEnterpriseProducts(enterpriseId);
                foreach (var product in products)
                {
                    ProductEarningsDataset productEarnings = new ProductEarningsDataset
                    {
                        ProductLabel = product.Name,
                        EarningsPerMonth = new List<double>(new double[12])
                    };
                    for (int i = 0; i < 12; i++)
                    {
                        productEarnings.EarningsPerMonth[i] = await _orderRepository.GetProductEarningsByMonth(product.Id, i);
                    }
                    earnings.Add(productEarnings);
                }
                return earnings;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw new Exception("Error al obtener las ganancias de la empresa");
            }
        }
    }
}

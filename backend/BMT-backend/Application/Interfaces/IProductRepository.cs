using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> CreateBaseProductAsync(Product product);
        Task<bool> CreateNonPerishableProductAsync(Product product);
        Task<bool> CreatePerishableProductAsync(Product product);

        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task<Product> GetProductsDetailsAsync();
        Task<Product> GetProductDetailsByIdAsyn(string id);
        Task<string> GetProductTypeAsync(string id);

        Task<int> GetProductStock(GetProductStockRequest product);

        Task<bool> UpdateProductAsync(Product product);
        Task<bool> UpdateProductStock(UpdateProductStockRequest product);
    }
}

using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> CreateBaseProductAsync(Product product);
        Task<bool> CreateNonPerishableProductAsync(string id, int stock);
        Task<bool> CreatePerishableProductAsync(string id, int limit, string weekDaysAvailable);

        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task<List<Product>> GetProductsDetailsAsync();
        Task<Product> GetProductDetailsByIdAsync(string id);
        Task<int> GetNonPerishableStock(string id);
        Task<int> GetPerishableStock(string id, string date);

        Task<bool> UpdateProductAsync(Product product);
        Task<bool> UpdateNonPerishableDetailsAsync(string id, int quantity);
        Task<bool> UpdatePerishableDetailsAsync(string id, string date, int? quantity);
        Task<bool> UpdateNonPerishableStockAsync(string productId, int quantity);
        Task<bool> UpdatePerishableStockAsync(string productId, string date, int quantity);
        Task<List<string>> SearchProductsIdAsync(string searchTerm);
    }
}

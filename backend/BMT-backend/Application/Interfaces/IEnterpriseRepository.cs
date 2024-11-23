using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Presentation.DTOs;

namespace BMT_backend.Application.Interfaces
{
    public interface IEnterpriseRepository
    {
        Task<bool> CreateEnterpriseAsync(Enterprise enterprise);
        Task<List<Enterprise>> GetEnterprisesAsync();
        Task<Enterprise> GetEnterpriseByIdAsync(string enterpriseId);
        Task<Entrepreneur> GetEnterpriseAdministratorAsync(string enterpriseId);
        Task<List<Entrepreneur>> GetEnterpriseStaffAsync(string enterpriseId);
        Task<int> GetProductsQuantityAsync(string id);
        Task<List<string>> GetEnterpriseProductsIdAsync(string enterpriseId);
        Task<string> CheckExistingEnterpriseAsync(Enterprise enterprise);
        Task<bool> UpdateEnterpriseAsync(UpdateEnterpriseRequest updatedEnterprise, List<string> fieldsToUpdate);
        Task<List<string>> SearchEnterprisesIdAsync(string searchTerm);
    }
}

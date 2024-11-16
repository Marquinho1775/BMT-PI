using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Domain.Views;

namespace BMT_backend.Application.Interfaces
{
    public interface IEnterpriseRepository
    {
        Task<bool> CheckIfEntryInTable(string tableName, string columnName, string columnValue);

        Task<bool> CreateEnterprise(Enterprise enterprise);

        Task<List<Enterprise>> GetEnterprises();

        Task<List<Enterprise>> GetEnterpriseStaff(string enterpriseId);

        Task<Entrepreneur> GetEnterpriseAdministrator(string enterpriseId);

        Task<List<DeveloperEnterpriseView>> GetDevEnterprises();

        Task<Enterprise>? GetEnterpriseById(string enterpriseId);

        Task<bool> UpdateEnterpriseProfile(UpdateEnterpriseRequest updatedEnterprise);
    }
}

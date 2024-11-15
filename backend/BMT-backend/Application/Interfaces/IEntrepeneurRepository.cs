using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IEntrepeneurRepository
    {
        Task<bool> CheckIfEntryInTable(string tableName, string columnName, string columnValue);
        Task<bool> CreateEntrepreneur(Entrepreneur entrepreneur);
        Task<bool> AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request);
        Task<List<Entrepreneur>> GetEntrepreneurs();
        Task<List<Enterprise>> GetEnterprisesOfEntrepreneur(string Identification);
        Task<Entrepreneur> GetEntrepreneurByUserId(string id);
    }
}

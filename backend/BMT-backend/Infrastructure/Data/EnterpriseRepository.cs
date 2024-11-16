//using BMT_backend.Application.Interfaces;
//using BMT_backend.Domain.Entities;
//using BMT_backend.Domain.Views;
//using System.Data.SqlClient;
//using System.Text;

//namespace BMT_backend.Infrastructure.Data
//{
//    public class EnterpriseRepository : IEnterpriseRepository
//    {
//        public async Task<bool> CheckIfEntryInTable(string tableName, string columnName, string columnValue);

//        public async Task<bool> CreateEnterprise(Enterprise enterprise);

//        public async Task<List<Enterprise>> GetEnterprises();

//        public async Task<List<Enterprise>> GetEnterpriseStaff(string enterpriseId);

//        public async Task<Entrepreneur> GetEnterpriseAdministrator(string enterpriseId);

//        public async Task<List<DeveloperEnterpriseView>> GetDevEnterprises();

//        public async Task<Enterprise>? GetEnterpriseById(string enterpriseId);

//        public async Task<bool> UpdateEnterpriseProfile(UpdateEnterpriseRequest updatedEnterprise);
//    }
//}
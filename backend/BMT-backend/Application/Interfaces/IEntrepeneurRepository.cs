﻿using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Application.Interfaces
{
    public interface IEntrepeneurRepository
    {
        Task<bool> CheckIfEntryInTable(string tableName, string columnName, string columnValue);
        Task<bool> CreateEntrepreneur(string UserId, string Identification);
        Task<bool> AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request);
        Task<List<Entrepreneur>> GetEntrepreneurs();
        Task<List<Enterprise>> GetEntrepreneurEnterprises(string Identification);
        Task<Entrepreneur> GetEntrepreneurByUserId(string id);
    }
}

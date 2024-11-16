using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Text.RegularExpressions;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Presentation.Requests;
using BMT_backend.Infrastructure.Data;

namespace BMT_backend.Application.Services
{
    public class EntrepeneurService
    {

        private readonly IEntrepeneurRepository _entrepeneurRepository;

        public EntrepeneurService(IEntrepeneurRepository entrepeneurRepository)
        {
            _entrepeneurRepository = entrepeneurRepository;
        }

        public async Task<bool> CreateEntrepreneur(Entrepreneur entrepreneur)
        {
            if (await _entrepeneurRepository.CheckIfEntryInTable("Entrepreneurs", "Identification", entrepreneur.Identification))
            {
                return false;
            }
            return await _entrepeneurRepository.CreateEntrepreneur(entrepreneur);
        }

        public async Task<bool> AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request)
        {
            return await _entrepeneurRepository.AddEntrepreneurToEnterprise(request);
        }

        public async Task<List<Entrepreneur>> GetEntrepreneurs()
        {
            return await _entrepeneurRepository.GetEntrepreneurs();
        }

        public async Task<List<Enterprise>> GetEnterprisesOfEntrepreneur(string identification)
        {
            return await _entrepeneurRepository.GetEntrepreneurEnterprises(identification);
        }

        public async Task<Entrepreneur> GetEntrepreneurByUserId(string id)
        {
            return await _entrepeneurRepository.GetEntrepreneurByUserId(id);
        }
    }
}

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
        private UserService _userService;

        public EntrepeneurService(IEntrepeneurRepository entrepeneurRepository, UserService userService)
        {
            _entrepeneurRepository = entrepeneurRepository;
            _userService = userService;
        }

        public async Task<bool> CreateEntrepreneur(string userId, string entrepreneurId)
        {
            if (await _entrepeneurRepository.CheckIfEntryInTable("Entrepreneurs", "Identification", entrepreneurId))
            {
                throw new System.Exception("El emprendedor ya existe");
            }
            await _entrepeneurRepository.CreateEntrepreneur(userId, entrepreneurId);
            return await _userService.UpdateRoleAsync(userId, "emp");
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

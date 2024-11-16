using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Text.RegularExpressions;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Presentation.Requests;
using BMT_backend.Infrastructure.Data;

namespace BMT_backend.Application.Services
{
    public class EnterpriseService
    {
        private readonly IEnterpriseRepository _enterpriseRepository;

        public EnterpriseService(IEnterpriseRepository enterpriseRepository)
        {
            _enterpriseRepository = enterpriseRepository;
        }



    }
}
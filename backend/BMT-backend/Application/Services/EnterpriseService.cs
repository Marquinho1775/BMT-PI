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
        private ProductService _productService;

        public EnterpriseService(IEnterpriseRepository enterpriseRepository, ProductService productService)
        {
            _enterpriseRepository = enterpriseRepository;
            _productService = productService;
        }

        public async Task<bool> CreateEnterpriseAsync(Enterprise enterprise)
        {
            ValidateEnterpriseData(enterprise);
            string existingEnterprise = await _enterpriseRepository.CheckExistingEnterpriseAsync(enterprise);
            if (existingEnterprise == "IdentificationNumber")
                throw new ArgumentException("El número de identificación ya está en uso.");
            else if (existingEnterprise == "Name")
                throw new ArgumentException("El nombre de la empresa ya está en uso.");
            else if (existingEnterprise == "Email")
                throw new ArgumentException("El correo electrónico ya está en uso.");
            else if (existingEnterprise == "PhoneNumber")
                throw new ArgumentException("El número de teléfono ya está en uso.");
            return await _enterpriseRepository.CreateEnterpriseAsync(enterprise);
        }

        public async Task<List<Enterprise>> GetAllEnterprisesAsync()
        {
            List<Enterprise> enterprises = await _enterpriseRepository.GetEnterprisesAsync();
            foreach(Enterprise enterprise in enterprises)
            {
                enterprise.Administrator = await _enterpriseRepository.GetEnterpriseAdministratorAsync(enterprise.Id);
                enterprise.Staff = await _enterpriseRepository.GetEnterpriseStaffAsync(enterprise.Id);
            }
            return enterprises;
        }

        public async Task<Enterprise> GetEnterpriseByIdAsync(string id)
        {
            Enterprise enterprise = await _enterpriseRepository.GetEnterpriseByIdAsync(id);
            enterprise.Administrator = await _enterpriseRepository.GetEnterpriseAdministratorAsync(id);
            enterprise.Staff = await _enterpriseRepository.GetEnterpriseStaffAsync(id);
            return enterprise;
        }

        public async Task<List<EnterpriseDevDto>> GetAllEnterpriseDevAsnc()
        {
            List<EnterpriseDevDto> enterpriseDevDtos = new List<EnterpriseDevDto>();
            List<Enterprise> enterprises = await GetAllEnterprisesAsync();
            foreach (Enterprise enterprise in enterprises)
            {
                EnterpriseDevDto enterpriseDevDto = new()
                {
                    Administrator = enterprise.Administrator.Name + " " + enterprise.Administrator.LastName,
                    Description = enterprise.Description,
                    Email = enterprise.Email,
                    EmployeeQuantity = enterprise.Staff.Count,
                    Name = enterprise.Name,
                    PhoneNumber = enterprise.PhoneNumber,
                    ProductQuantity = await _enterpriseRepository.GetProductsQuantityAsync(enterprise.Id),

                };
                enterpriseDevDtos.Add(enterpriseDevDto);
            }
            return enterpriseDevDtos;
        }

        public async Task<List<Product>> GetEnterpriseProductsDetails(string enterpriseId)
        {
            List<string> productsId =  await _enterpriseRepository.GetEnterpriseProductsIdAsync(enterpriseId);
            List<Product> products = [];
            foreach (string productId in productsId)
            {
                Product product = await _productService.GetProductDetailsByIdAsync(productId);
                products.Add(product);
            }
            return products;
        }

        public async Task<List<Product>> GetEnterpriseProducts(string enterpriseId)
        {
            List<string> productsId = await _enterpriseRepository.GetEnterpriseProductsIdAsync(enterpriseId);
            List<Product> products = [];
            foreach (string productId in productsId)
            {
                Product product = await _productService.GetProductByIdAsync(productId);
                products.Add(product);
            }
            return products;
        }

        public async Task<bool> UpdateEnterpriseAsync(UpdateEnterpriseRequest updatedEnterprise)
        {
            // validar que el nuevo nombre de empresa no esté en uso
            var fieldsToUpdate = new List<string>();
            if (!string.IsNullOrEmpty(updatedEnterprise.Name))
            {
                fieldsToUpdate.Add("Name = @Name");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.Description))
            {
                fieldsToUpdate.Add("Description = @Description");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.Email))
            {
                fieldsToUpdate.Add("Email = @Email");
            }
            if (!string.IsNullOrEmpty(updatedEnterprise.PhoneNumber))
            {
                fieldsToUpdate.Add("PhoneNumber = @PhoneNumber");
            }
            if (fieldsToUpdate.Count == 0)
            {
                throw new ArgumentException("No hay campos válidos para actualizar.");
            }
            return await _enterpriseRepository.UpdateEnterpriseAsync(updatedEnterprise, fieldsToUpdate);
        }

        private static void ValidateEnterpriseData(Enterprise enterprise)
        {
            if (enterprise == null)
                throw new ArgumentException("La información de la empresa es obligatoria.");

            if (string.IsNullOrWhiteSpace(enterprise.Name))
                throw new ArgumentException("El nombre de la empresa es obligatorio.");

            if (string.IsNullOrWhiteSpace(enterprise.Description))
                throw new ArgumentException("La descripción de la empresa es obligatoria.");

            if (string.IsNullOrWhiteSpace(enterprise.Email))
                throw new ArgumentException("El correo electrónico de la empresa es obligatorio.");

            if (!IsValidEmail(enterprise.Email))
                throw new ArgumentException("El formato del correo electrónico es inválido.");

            if (string.IsNullOrWhiteSpace(enterprise.PhoneNumber))
                throw new ArgumentException("El número de teléfono de la empresa es obligatorio.");

            if (!IsValidPhoneNumber(enterprise.PhoneNumber))
                throw new ArgumentException("El número de teléfono debe contener exactamente 8 dígitos numéricos.");

            if (string.IsNullOrWhiteSpace(enterprise.IdentificationNumber))
                throw new ArgumentException("El número de identificación es obligatorio.");

            if (!IsValidIdentificationNumber(enterprise.IdentificationNumber, enterprise.IdentificationType))
                throw new ArgumentException("El número de identificación no es válido para el tipo de identificación proporcionado.");

            if (enterprise.IdentificationType != 1 && enterprise.IdentificationType != 2)
                throw new ArgumentException("El tipo de identificación debe ser 1 o 2.");
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{8}$");
        }

        private static bool IsValidIdentificationNumber(string idNumber, int idType)
        {
            if (idType == 1)
            {
                return Regex.IsMatch(idNumber, @"^\d{9}$");
            }
            else if (idType == 2)
            {
                return Regex.IsMatch(idNumber, @"^\d{10}$");
            }
            else
            {
                return false;
            }
        }

        public async Task<List<YearlyEarningsReportDataDto>> GetYearlyEnterpriseDataAsync(YearlyEarningsReportDataRequest request)
        {
            if (!YearlyEarningsReportValidation(request))
                throw new ArgumentException("Los datos del reporte anual no son válidos.");

            var data = await _enterpriseRepository.GetYearlyEnterpriseDataAsync(request.EnterpriseIds, request.Year);

            return data;
        }

        public bool YearlyEarningsReportValidation(YearlyEarningsReportDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.EnterpriseIds))
                return false;

            if (request.Year < 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteEnterpriseAsync(string enterpriseId)
        {
            if (string.IsNullOrEmpty(enterpriseId))
            {
                throw new ArgumentException("El ID de la empresa no puede ser nulo o vacío.");
            }

            return await _enterpriseRepository.DeleteEnterpriseAsync(enterpriseId);
        }
    }
}
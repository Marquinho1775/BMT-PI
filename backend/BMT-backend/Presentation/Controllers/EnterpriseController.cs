using Microsoft.AspNetCore.Mvc;
using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Application.Queries;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseService _enterpriseService;
        private readonly GetEnterpriseEarningsQuery _getEnrerpriseEarningsQuery;

        public EnterpriseController(EnterpriseService enterpriseService, GetEnterpriseEarningsQuery getEnrerpriseEarningsQuery)
        {
            _enterpriseService = enterpriseService;
            _getEnrerpriseEarningsQuery = getEnrerpriseEarningsQuery;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnterprise([FromBody] Enterprise enterprise)
        {
            if (enterprise == null)
                return BadRequest(new { Message = "La información de la empresa no puede ser nula." });
            try
            {
                var result = await _enterpriseService.CreateEnterpriseAsync(enterprise);
                if (result)
                    return Ok(new { Success = true, Message = "Empresa creada exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo crear la empresa." });
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Ocurrió un error interno del servidor." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEnterprises()
        { 
            try
            {
                var enterprises = await _enterpriseService.GetAllEnterprisesAsync();
                return Ok(new { Success = true, Data = enterprises });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor." });
            }
        }

        [HttpGet("GetEnterpriseById")]
        public async Task<IActionResult> GetEnterpriseById([FromQuery] string enterpriseId)
        {
            if (string.IsNullOrWhiteSpace(enterpriseId))
                return BadRequest(new { Message = "El ID de la empresa es obligatorio." });
            try
            {
                var enterprise = await _enterpriseService.GetEnterpriseByIdAsync(enterpriseId);
                if (enterprise != null)
                    return Ok(new { Success = true, Data = enterprise });
                else
                    return NotFound(new { Success = false, Message = "Empresa no encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al obtener la empresa." });
            }
        }

        [HttpGet("GetEnterpriseProducts")]
        public async Task<IActionResult> GetEnterpriseProducts([FromQuery] string enterpriseId)
        {
            if (string.IsNullOrWhiteSpace(enterpriseId))
                return BadRequest(new { Message = "El ID de la empresa es obligatorio." });
            try
            {
                var products = await _enterpriseService.GetEnterpriseProductsDetails(enterpriseId);
                return Ok(new { Success = true, Data = products });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al obtener los productos de la empresa." });
            }
        }

        [HttpGet("GetEnterpriseEarnings")]
        public async Task<IActionResult> GetEnterpriseEarnings(string enterpriseId)
        {
            if (string.IsNullOrWhiteSpace(enterpriseId))
                return BadRequest(new { Message = "El ID de la empresa es obligatorio." });
            try
            {
                var earnings = await _getEnrerpriseEarningsQuery.GetEnterpriseEarningsAsync(enterpriseId);
                return Ok(new { Success = true, Data = earnings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al obtener las ganancias de la empresa." });
            }
        }

        [HttpPut("UpdateEnterprise")]
        public async Task<IActionResult> UpdateEnterprise([FromBody] UpdateEnterpriseRequest updatedEnterprise)
        {
            if (updatedEnterprise == null)
                return BadRequest(new { Message = "Los datos de la empresa actualizados no pueden ser nulos." });
            try
            {
                var result = await _enterpriseService.UpdateEnterpriseAsync(updatedEnterprise);
                if (result)
                    return Ok(new { Success = true, Message = "Empresa actualizada exitosamente." });
                else
                    return NotFound(new { Success = false, Message = "Empresa no encontrada o no se pudo actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error actualizando la empresa." });
            }
        }
    }
}

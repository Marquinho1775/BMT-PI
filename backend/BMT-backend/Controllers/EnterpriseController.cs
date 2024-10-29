using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly EnterpriseHandler _entrepeneurshipHandler;

        public EnterpriseController()
        {
            _entrepeneurshipHandler = new EnterpriseHandler();
        }

        [HttpGet]
        public List<EnterpriseModel> Get()
        {
            return _entrepeneurshipHandler.GetEnterprises();
        }

        [HttpGet("CheckExistingEnterprise")]
        public async Task<ActionResult<bool>> CheckExistingEnterprise(string identification)
        {
            try
            {
                if (string.IsNullOrEmpty(identification))
                {
                    return BadRequest("Identification cannot be null or empty.");
                }
                var result = _entrepeneurshipHandler.CheckIfEnterpriseExists(identification);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si la empresa existe");
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateEnterprise(EnterpriseModel enterprise)
        {
            try
            {
                if (enterprise == null)
                {
                    return BadRequest();
                }
                var result = _entrepeneurshipHandler.CreateEnterprise(enterprise);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando la empresa");
            }
        }


        [HttpGet("{enterpriseId}")]
        public ActionResult<EnterpriseModel> GetEnterpriseById(string enterpriseId)
        {
            try
            {
                var enterprise = _entrepeneurshipHandler.GetEnterpriseById(enterpriseId);

                if (enterprise == null)
                {
                    return NotFound("Empresa no encontrada");
                }

                return Ok(enterprise);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la empresa");
            }
        }

        [HttpPut("{enterpriseId}")]
        public async Task<ActionResult<bool>> UpdateEnterprise(string enterpriseId, UpdateEnterpriseModel updatedEnterprise)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseId) || updatedEnterprise == null)
                {
                    return BadRequest("Id de la empresa y datos de actualización son requeridos.");
                }

                updatedEnterprise.Id = enterpriseId;
                var result = _entrepeneurshipHandler.UpdateEnterpriseProfile(updatedEnterprise);

                if (!result)
                {
                    return NotFound("Empresa no encontrada o no se pudo actualizar.");
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando la empresa");
            }
        }

    }
}

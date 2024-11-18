
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using BMT_backend.Application.Services;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntrepreneurController : ControllerBase
    {
        private readonly EntrepeneurService _entrepeneurService;
        public EntrepreneurController(EntrepeneurService entrepeneurService)
        {
            _entrepeneurService = entrepeneurService;
        }

        [HttpGet]
        public async Task<List<Entrepreneur>> Get()
        {
            var entrepreneurs = await _entrepeneurService.GetEntrepreneurs();
            return entrepreneurs;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateEntrepreneur(CreateEntrepreneurRequest entrepreneur)
        {
            try
            {
                if (entrepreneur == null
                    || string.IsNullOrEmpty(entrepreneur.UserId) || string.IsNullOrEmpty(entrepreneur.Identification))
                {
                    return BadRequest("Username and Identification cannot be null or empty.");
                }
                var result = await _entrepeneurService.CreateEntrepreneur(entrepreneur.UserId, entrepreneur.Identification);
                return Ok(new { Success = result });
            }
            catch (Exception ex) when (ex.Message.Contains("El emprendedor ya existe"))
            {
                return Conflict("Ya existe un emprendedor con este número de identificación.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando al emprendedor");
            }
        }

        [HttpPost("add-to-enterprise")]
        public async Task<ActionResult<bool>> AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request)
        {
            try
            {
                if (request == null
                    || string.IsNullOrEmpty(request.EntrepreneurIdentification) || string.IsNullOrEmpty(request.EnterpriseIdentification))
                {
                    return BadRequest("Identifications cannot be null or empty.");
                }
                var result = await _entrepeneurService.AddEntrepreneurToEnterprise(request);
                return Ok(new { Success = result });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error añadiendo al emprendedor a la empresa");
            }
        }

        [HttpGet("my-registered-enterprises")]
        public async Task<ActionResult<bool>> ConsultRegisteredEnterprises(string Identification)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = await _entrepeneurService.GetEnterprisesOfEntrepreneur(Identification);
                return Ok(new { Success = result }); 

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error consultando las empresas registradas");
            }
        }

        [HttpGet("GetEntrepreneurByUserId")]
        public async Task<ActionResult<bool>> GetEntrepreneurByUserId(string id)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = await _entrepeneurService.GetEntrepreneurByUserId(id);
                return Ok(result);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo el emprendedor");
            }
        }


    }
}

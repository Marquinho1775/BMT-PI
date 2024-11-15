
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

        [HttpGet("CheckExistingEntrepreneur")]
        public async Task<ActionResult<bool>> CheckExistingEntrepreneur(string identification)
        {
            try
            {
                if (string.IsNullOrEmpty(identification))
                {
                    return BadRequest("Identification cannot be null or empty.");
                }
                var result = _entrepeneurService.CheckIfEntrepreneurExists(identification);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si el emprendedor existe");
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateEntrepreneur(Entrepreneur entrepreneur)
        {
            try
            {
                if (entrepreneur == null
                    || string.IsNullOrEmpty(entrepreneur.Username) || string.IsNullOrEmpty(entrepreneur.Identification))
                {
                    return BadRequest("Username and Identification cannot be null or empty.");
                }
                var result = _entrepeneurService.CreateEntrepreneur(entrepreneur);
                return new JsonResult(result);
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
                var result = _entrepeneurService.AddEntrepreneurToEnterprise(request);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error añadiendo al emprendedor a la empresa");
            }
        }

        [HttpPost("my-registered-enterprises")]
        public async Task<ActionResult<bool>> ConsultRegisteredEnterprises(string Identification)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = _entrepeneurService.GetEnterprisesOfEntrepreneur(Identification);
                return new JsonResult(result);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error consultando las empresas registradas");
            }
        }

        [HttpPost("GetEntrepreneurByUserId")]
        public async Task<ActionResult<bool>> GetEntrepreneurByUserId(string id)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = _entrepeneurService.GetEntrepreneurByUserId(id);
                return new JsonResult(result);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo el emprendedor");
            }
        }


    }
}

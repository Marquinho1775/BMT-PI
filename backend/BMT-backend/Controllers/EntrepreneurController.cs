using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntrepreneurController : ControllerBase
    {
        private readonly EntrepreneurHandler _entrepreneurHandler;
        public EntrepreneurController()
        {
            _entrepreneurHandler = new EntrepreneurHandler();
        }

        [HttpGet]
        public List<EntrepreneurModel> Get()
        {
            var entrepreneurs = _entrepreneurHandler.GetEntrepreneurs();
            return entrepreneurs;
        }

        // IMPORTANTE: Pasar el modelo completo de Entrepreneur (User + Identification)
        [HttpPost]
        public async Task<ActionResult<bool>> CreateEntrepreneur(EntrepreneurModel entrepreneur)
        {
            try
            {
                if (entrepreneur == null)
                {
                    return BadRequest();
                }
                var result = _entrepreneurHandler.CreateEntrepreneur(entrepreneur);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando al emprendedor");
            }
        }
    }
}

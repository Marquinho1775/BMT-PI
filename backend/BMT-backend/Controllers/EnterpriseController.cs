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
    }
}

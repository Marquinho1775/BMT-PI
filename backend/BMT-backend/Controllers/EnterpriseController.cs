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
            var enterprises = _entrepeneurshipHandler.GetEnterprises();
            return enterprises;
        }
    }
}

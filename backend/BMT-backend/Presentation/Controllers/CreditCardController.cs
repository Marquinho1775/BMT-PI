using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BMT_backend.Handlers;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        private readonly CreditCardHandler _creditCardHandler;

        public CreditCardController(IConfiguration configuration)
        {
            _creditCardHandler = new CreditCardHandler(configuration);
        }


        [HttpPost]
        public async Task<ActionResult<bool>> CreateCreditCard(CreditCard creditCard)
        {
            try
            {
                if (creditCard == null)
                {
                    return BadRequest();
                }
                var result = _creditCardHandler.CreateCreditCard(creditCard);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the credit card");
            }
        }

        [HttpGet]
        public async Task<List<CreditCard>> GetCreditCards()
        {
            var creditCards = await Task.Run(() => _creditCardHandler.GetCreditCards());
            return creditCards;
        }

        [HttpGet("User")]
        public async Task<List<CreditCard>> GetCreditCardsByUser(string userId)
        {
            var creditCards = await Task.Run(() => _creditCardHandler.GetCreditCardsByUser(userId));
            return creditCards;
        }
    }
}

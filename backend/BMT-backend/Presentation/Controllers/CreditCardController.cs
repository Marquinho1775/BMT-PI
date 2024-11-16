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
        private readonly CreditCardService _creditCardService;

        public CreditCardController(IConfiguration configuration, CreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
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
                var result = await _creditCardService.CreateCreditCardAsync(creditCard);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the credit card");
            }
        }

        [HttpGet]
        public async Task<List<CreditCard>> GetCreditCards()
        {
            var creditCards = await _creditCardService.GetCreditCardsAsync();
            return creditCards;
        }

        [HttpGet("User")]
        public async Task<List<CreditCard>> GetCreditCardsByUser(string userId)
        {
            var creditCards = await _creditCardService.GetCreditCardsByUserIdAsync(userId);
            return creditCards;
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BMT_backend.Handlers;
using BMT_backend.Models;

namespace BMT_backend.Controllers
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
        public async Task<ActionResult<bool>> CreateCreditCard(CreditCardModel creditCard)
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
        public List<CreditCardModel> GetCreditCards()
        {
            var creditCards = _creditCardHandler.GetCreditCards();
            return creditCards;
        }

        [HttpGet("User")]
        public List<CreditCardModel> GetCreditCardsByUser(string userId)
        {
            var creditCards = _creditCardHandler.GetCreditCardsByUser(userId);
            return creditCards;
        }
    }
}
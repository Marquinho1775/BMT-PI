﻿using BMT_backend.Models;
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

        [HttpGet] public List<EntrepreneurViewModel> Get()
        {
            var entrepreneurs = _entrepreneurHandler.GetEntrepreneurs();
            return entrepreneurs;
        }

        [HttpPost] public async Task<ActionResult<bool>> CreateEntrepreneur(EntrepreneurModel entrepreneur)
        {
            try
            {
                if (entrepreneur == null
                    || string.IsNullOrEmpty(entrepreneur.Username) || string.IsNullOrEmpty(entrepreneur.Identification))
                {
                    return BadRequest ("Username and Identification cannot be null or empty.");
                }
                var result = _entrepreneurHandler.CreateEntrepreneur(entrepreneur);
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
                var result = _entrepreneurHandler.AddEntrepreneurToEnterprise(request);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error añadiendo al emprendedor a la empresa");
            }
        }
    }
}
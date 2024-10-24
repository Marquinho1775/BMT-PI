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

        [HttpGet]
        public List<EntrepreneurViewModel> Get()
        {
            var entrepreneurs = _entrepreneurHandler.GetEntrepreneurs();
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
                var result = _entrepreneurHandler.CheckIfEntrepreneurExists(identification);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si el emprendedor existe");
            }
        }

        [HttpPost] 
        public async Task<ActionResult<bool>> CreateEntrepreneur(EntrepreneurModel entrepreneur)
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

        [HttpPost("my-registered-enterprises")]
        public async Task<ActionResult<bool>> ConsultRegisteredEnterprises(EntrepreneurModel entrepeneur)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = _entrepreneurHandler.GetEnterprisesOfEntrepreneur(entrepeneur);
                return new JsonResult(result);

            } catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error consultando las empresas registradas");
            }
        }

        [HttpPost("ObtainEntrepreneurBasedOnUser")]
        public async Task<ActionResult<bool>> EntrepreneurBasedOnUser(UserModel user)
        {
            try
            {
                if (Request == null)
                {
                    BadRequest();
                }

                var result = _entrepreneurHandler.GetEntrepreneurBasedOnAUser(user);
                return new JsonResult(result);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error consultando las empresas registradas");
            }
        }


    }
}

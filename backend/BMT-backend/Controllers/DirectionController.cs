using BMT_backend.Handlers;
using BMT_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectionController : ControllerBase
    {
        private readonly DirectionHandler _directionHandler;
        public DirectionController()
        {
            _directionHandler = new DirectionHandler();
        }

        [HttpPost("ObtainDirectionsFromUser")]
        public async Task<ActionResult<List<DirectionModel>>> getDirectionsFromUser([FromBody] UserModel user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is required");
                }

                var result = _directionHandler.GetDirectionsFromUser(user);
                return Ok(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error consultando las direcciones de un usuario");
            }
        }

        [HttpPost("CreateDirection")]
        public async Task<ActionResult<bool>> createDirection(DirectionModel direction)
        {
            try
            {
                if (Request == null)
                {
                    return BadRequest();
                }

                var result = _directionHandler.CreateDirection(direction);
                return new JsonResult(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error registrando direcciones");
            }
        }

        [HttpPut("UpdateDirection/{id}")]
        public async Task<ActionResult<bool>> UpdateDirection(string id, [FromBody] DirectionModel updatedDirection)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || updatedDirection == null)
                {
                    return BadRequest("ID y datos de actualización son requeridos.");
                }

                updatedDirection.Id = id;
                var result = _directionHandler.UpdateDirection(updatedDirection);

                if (!result)
                {
                    return NotFound("Dirección no encontrada o no se pudo actualizar.");
                }

                return Ok(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando la dirección");
            }
        }
    }
}

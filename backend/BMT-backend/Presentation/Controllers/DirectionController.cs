using Microsoft.AspNetCore.Mvc;
using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;


namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectionController : ControllerBase
    {
        private readonly DirectionService _directionService;

        public DirectionController(DirectionService directionService)
        {
            _directionService = directionService;
        }

        [HttpGet("GetDirectionsFromUser")]
        public async Task<IActionResult> GetDirectionsFromUser([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { Success = false, Message = "El nombre de usuario es obligatorio." });

            try
            {
                var user = new User { Username = username };
                var directions = await _directionService.GetDirectionsFromUserAsync(user);

                if (directions != null && directions.Count > 0)
                    return Ok(new { Success = true, Data = directions });
                else
                    return NotFound(new { Success = false, Message = "No se encontraron direcciones para el usuario especificado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al obtener las direcciones.", Details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDirection([FromBody] Direction direction)
        {
            if (direction == null)
                return BadRequest(new { Success = false, Message = "La información de la dirección no puede ser nula." });

            try
            {
                var result = await _directionService.CreateDirectionAsync(direction);
                if (result)
                    return Ok(new { Success = true, Message = "Dirección creada exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo crear la dirección." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al crear la dirección.", Details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDirection([FromBody] Direction direction)
        {
            if (direction == null)
                return BadRequest(new { Success = false, Message = "La información de la dirección no puede ser nula." });

            try
            {
                var result = await _directionService.UpdateDirectionAsync(direction);
                if (result)
                    return Ok(new { Success = true, Message = "Dirección actualizada exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo actualizar la dirección." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al actualizar la dirección.", Details = ex.Message });
            }
        }
    }
}

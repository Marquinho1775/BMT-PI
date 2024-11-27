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
        public async Task<IActionResult> GetDirectionsFromUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { Success = false, Message = "El nombre de usuario es obligatorio." });

            try
            {
                var directions = await _directionService.GetDirectionsFromUserAsync(id);

                if (directions != null && directions.Count >= 0)
                    return Ok(new { Success = true, Data = directions });
                else
                    return NotFound(new { Success = false, Message = "Internal server error." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al obtener las direcciones.", Details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDirection(Direction direction)
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
        public async Task<IActionResult> UpdateDirection(Direction direction)
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

        [HttpDelete("Delete/{directionId}")]
        public async Task<IActionResult> DeleteDirection(string directionId)
        {
            if (string.IsNullOrEmpty(directionId))
                return BadRequest(new { Success = false, Message = "El identificador de la dirección es obligatorio." });

            try
            {
                var result = await _directionService.DeleteDirectionAsync(directionId);
                if (result)
                    return Ok(new { Success = true, Message = "Dirección eliminada." });
                else
                    return NotFound(new { Success = false, Message = "La dirección no pudo ser encontrada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al eliminar la dirección.", Details = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _productTagService;

        public TagController(TagService productTagService)
        {
            _productTagService = productTagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                var tags = await _productTagService.GetTags();
                return Ok(new { Success = true, Data = tags });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al obtener las etiquetas.", Details = ex.Message });
            }
        }

        [HttpGet("GetProductTags")]
        public async Task<IActionResult> GetProductTagsById([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { Success = false, Message = "El ID del producto es obligatorio." });
            try
            {
                var tags = await _productTagService.GetProductTagsById(id);
                if (tags != null && tags.Any())
                    return Ok(new { Success = true, Data = tags });
                else
                    return NotFound(new { Success = false, Message = "No se encontraron etiquetas para el producto especificado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al obtener las etiquetas del producto.", Details = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] Tag productTag)
        {
            if (productTag == null)
                return BadRequest(new { Success = false, Message = "La información de la etiqueta no puede ser nula." });

            try
            {
                var result = await _productTagService.CreateTag(productTag);
                if (result)
                    return Ok(new { Success = true, Message = "Etiqueta creada exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo crear la etiqueta." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al crear la etiqueta.", Details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductTag([FromBody] Tag productTag)
        {
            if (productTag == null)
                return BadRequest(new { Success = false, Message = "La información de la etiqueta no puede ser nula." });

            try
            {
                var result = await _productTagService.UpdateProductTag(productTag);
                if (result)
                    return Ok(new { Success = true, Message = "Etiqueta actualizada exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo actualizar la etiqueta." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al actualizar la etiqueta.", Details = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductTag([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { Success = false, Message = "El nombre de la etiqueta es obligatorio." });

            try
            {
                var result = await _productTagService.DeleteProductTag(name);
                if (result)
                    return Ok(new { Success = true, Message = "Etiqueta eliminada exitosamente." });
                else
                    return NotFound(new { Success = false, Message = "No se encontró la etiqueta especificada para eliminar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor al eliminar la etiqueta.", Details = ex.Message });
            }
        }
    }
}

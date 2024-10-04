using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageUploadHandler _imgageUploadHandler;
        
        public ImageUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _imgageUploadHandler = new ImageUploadHandler();
        }

        [HttpPost("upload")]
        public async Task<ActionResult<bool>> UploadImage(IFormFile image, string ownerId)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Debe proporcionar una imagen válida.");
            }
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                var relativePath = Path.Combine("uploads", uniqueFileName);
                _imgageUploadHandler.SaveImage(ownerId, relativePath);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error guardando la imagen: {ex.Message}");
            }
        }
    }
}

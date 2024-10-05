using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageFileHandler _imageUploadHandler;
        
        public ImageFileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _imageUploadHandler = new ImageFileHandler();
        }

        [HttpPost("upload")]
        public async Task<ActionResult<bool>> UploadImage(ImageFileModel imageFile)
        {
            if (imageFile.Image == null || imageFile.Image.Length == 0)
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
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.Image.CopyToAsync(fileStream);
                }
                var relativePath = Path.Combine("uploads", uniqueFileName);
                _imageUploadHandler.SaveImage(imageFile.OwnerId, imageFile.OwnerType, relativePath);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error guardando la imagen: {ex.Message}");
            }
        }

        [HttpGet("get")]
        public ActionResult<List<string>> GetImages(string ownerId)
        {
            try
            {
                var images = _imageUploadHandler.GetImage(ownerId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error obteniendo las imágenes: {ex.Message}");
            }
        }
    }
}

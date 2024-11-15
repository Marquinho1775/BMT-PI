using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageFileHandler _imageFileHandler;

        public ImageFileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _imageFileHandler = new ImageFileHandler();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages([FromForm] string ownerId, [FromForm] string ownerType, [FromForm] List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
            {
                return BadRequest("No images were provided.");
            }
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    var relativePath = "uploads/" + fileName;
                    _imageFileHandler.SaveImage(ownerId, ownerType, relativePath);
                }
            }
            return Ok("Images uploaded successfully.");
        }

        [HttpGet("get-product-images")]
        public ActionResult<List<string>> GetProductImages(string productId)
        {
            try
            {
                var images = _imageFileHandler.GetProductImages(productId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error obteniendo las imágenes: {ex.Message}");
            }
        }

        [HttpGet("get-profile-image")]
        public ActionResult<string> GetProfileImage(string userId)
        {
            try
            {
                var image = _imageFileHandler.GetProfileImage(userId);
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error obteniendo la imagen de perfil: {ex.Message}");
            }
        }
    }
}

﻿using BMT_backend.Models;
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
                    var relativePath = Path.Combine("uploads", fileName);
                    _imageUploadHandler.SaveImage(ownerId, ownerType, relativePath);
                }
            }
            return Ok("Images uploaded successfully.");
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


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Application.Interfaces;
using BMT_backend.Handlers;

namespace BMT_backend.Infrastructure
{
    public class ImageFileService : IImageFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageFileRepository _imageFileRepository;

        public ImageFileService(IWebHostEnvironment webHostEnvironment, IImageFileRepository imageFileRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _imageFileRepository = imageFileRepository;
        }


        public bool CreateProductImages(string productId, List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
            {
                return false;
            }
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyToAsync(stream);
                    }
                    var relativePath = "uploads/" + fileName;
                    _imageFileRepository.CreateProductImageAsync(productId, relativePath);
                }
            }
            return true;
        }

        public bool UpdateProfileImage(string userId, FormFile image)
        {
            if (image.Length > 0)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyToAsync(stream);
                }
                var relativePath = "uploads/" + fileName;
                _imageFileRepository.UpdateProfileImageAsync(userId, relativePath);
                return true;
            }
            return false;
        }


        public async Task<List<string>> GetProductsImages(string productId)
        {
            return await GetProductsImages(productId);
        }

        public async Task<string> GetProfileImage(string userId)
        {
            return await GetProfileImage(userId);
        }
    }
}

using BMT_backend.Application.Interfaces;

namespace BMT_backend.Infrastructure
{
    public class ImageFileService : IImageFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageFileRepository _imageFileRepository;
        private readonly ILogger<ImageFileService> _logger;

        public ImageFileService(IWebHostEnvironment webHostEnvironment, IImageFileRepository imageFileRepository, ILogger<ImageFileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _imageFileRepository = imageFileRepository;
            _logger = logger;
        }

        public async Task<bool> CreateProductImages(string productId, List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
            {
                return false;
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
                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        var relativePath = Path.Combine("uploads", fileName);
                        await _imageFileRepository.CreateProductImageAsync(productId, relativePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing image {FileName}", image.FileName);
                        return false;
                    }
                }
            }
            return true;
        }


        public async Task<bool> UpdateProfileImage(string userId, FormFile image)
        {
            if (image.Length > 0)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                var relativePath = "uploads/" + fileName;
                await _imageFileRepository.UpdateProfileImageAsync(userId, relativePath);
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

namespace BMT_backend.Application.Interfaces
{
    public interface IImageFileService
    {
        Task<bool> CreateProductImages(string productId, List<IFormFile> images);
        Task<bool> UpdateProfileImage(string userId, FormFile image);
        Task<List<string>> GetProductsImages(string productId);
        Task<string> GetProfileImage(string userId);
    }
}

namespace BMT_backend.Application.Interfaces
{
    public interface IImageFileService
    {
        bool CreateProductImages(string productId, List<IFormFile> images);
        bool UpdateProfileImage(string userId, FormFile image);
        Task<List<string>> GetProductsImages(string productId);
        Task<string> GetProfileImage(string userId);
    }
}

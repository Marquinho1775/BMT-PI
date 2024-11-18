namespace BMT_backend.Application.Interfaces
{
    public interface IImageFileRepository
    {
        Task<bool> CreateProductImageAsync(string productId, string relativePath);
        Task<bool> UpdateProfileImageAsync(string userId, string relativePath);
        Task<List<string>> GetProductsImagesAsync(string productId);
        Task<string> GetProfileImageAsync(string userId);
    }
}

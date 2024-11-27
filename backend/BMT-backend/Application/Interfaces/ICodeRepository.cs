

namespace BMT_backend.Application.Interfaces
{
    public interface ICodeRepository
    {
        Task<string> CreateCodeAsync(string userId);
        Task<string> GetCodeAsync(string userId);
    }
}
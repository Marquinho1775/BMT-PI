using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}

using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface IDirectionRepository
    {
        Task<List<Direction>> GetDirectionsFromUserAsync(User user);
        Task<bool> CreateDirectionAsync(Direction direction);
        Task<bool> UpdateDirection(Direction direction);
    }
}

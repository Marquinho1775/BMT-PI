using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface IDirectionRepository
    {
        Task<List<Direction>> GetDirectionsFromUserAsync(string id);
        Task<bool> CreateDirectionAsync(Direction direction);
        Task<bool> UpdateDirectionAsync(Direction direction);
    }
}

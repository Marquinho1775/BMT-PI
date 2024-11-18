using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface IDirectionRepository
    {
        Task<List<Direction>> GetDirectionsFromUserAsync(string id);
        Task<bool> CreateDirectionAsync(Direction direction);
        Task<bool> UpdateDirectionAsync(Direction direction);
        Task<string> GetCoordinates(string directionId);
        Task<bool> SoftDeleteDirectionAsync(string directionId);
        Task<bool> HardDeleteDirectionAsync(string directionId);
    }
}

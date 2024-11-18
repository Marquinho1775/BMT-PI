using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BMT_backend.Application.Services
{
    public class DirectionService
    {
        private readonly IDirectionRepository _directionRepository;

        public DirectionService(IDirectionRepository directionRepository)
        {
            _directionRepository = directionRepository;
        }

        public async Task<List<Direction>> GetDirectionsFromUserAsync(string id)
        {
            return await _directionRepository.GetDirectionsFromUserAsync(id);
        }

        public async Task<bool> CreateDirectionAsync(Direction direction)
        {
            return await _directionRepository.CreateDirectionAsync(direction);
        }

        public async Task<bool> UpdateDirectionAsync(Direction direction)
        {
            return await _directionRepository.UpdateDirectionAsync(direction);
        }
        public async Task<string> GetCoordinates(string directionId)
        {
            return await _directionRepository.GetCoordinates(directionId);
        }

        public async Task<bool> SoftDeleteDirectionAsync(string directionId)
        {
            return await _directionRepository.SoftDeleteDirectionAsync(directionId);
        }

        public async Task<bool> HardDeleteDirectionAsync(string directionId)
        {
            return await _directionRepository.HardDeleteDirectionAsync(directionId);
        }
    }
}

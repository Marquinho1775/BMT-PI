using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Services

{
    public class TagService
    {
        private readonly ITagRepository _TagRepository;

        public TagService(ITagRepository productTag)
        {
            _TagRepository = productTag;
        }

        public async Task<List<Tag>> GetTags()
        {
            return await _TagRepository.GetTagsAsync();
        }

        public async Task<List<Tag>> GetProductTagsById(string id)
        {
            return await _TagRepository.GetProductTagsByIdAsync(id);
        }

        public async Task<bool> CreateTag(Tag productTag)
        {
            return await _TagRepository.CreateTagAsync(productTag);
        }

        public async Task<bool> UpdateProductTag(Tag productTag)
        {
            return await _TagRepository.UpdateTagAsync(productTag);
        }

        public async Task<bool> DeleteProductTag(string name)
        {
            return await _TagRepository.DeleteTagAsync(name);
        }
    }
}

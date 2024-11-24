using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.ComponentModel;

namespace BMT_backend.Application.Services

{
    public class TagService
    {
        private readonly ITagRepository _TagRepository;

        public TagService(ITagRepository TagRepository)
        {
            _TagRepository = TagRepository;
        }

        public async Task<List<Tag>> GetTags()
        {
            return await _TagRepository.GetTagsAsync();
        }

        public async Task<List<string>> GetTagsIdByTagsName(List<string> tagNames)
        {
            if (tagNames != null)
                return await _TagRepository.GetTagsIdByTagsNameAsync(tagNames);
            return [];
        }

        public async Task<List<Tag>> GetProductTagsById(string id)
        {
            return await _TagRepository.GetProductTagsByIdAsync(id);
        }

        public async Task<bool> CreateTag(Tag productTag)
        {
            return await _TagRepository.CreateTagAsync(productTag);
        }

        public async Task<bool> UpdateTag(Tag productTag)
        {
            return await _TagRepository.UpdateTagAsync(productTag);
        }

        public async Task<bool> DeleteTag(string name)
        {
            return await _TagRepository.DeleteTagAsync(name);
        }

        public async Task<bool> UpdateProductTags(string productId, List<string> updatedTagsId)
        {
            List<Tag> currentTags = await GetProductTagsById(productId);
            List<string> currentTagsId = currentTags.Select(t => t.Id).ToList();
            List<string> toDeleteTagsId = currentTagsId.Except(updatedTagsId).ToList();
            List<string> toAddTagsId = updatedTagsId.Except(currentTagsId).ToList();
            if (toAddTagsId.Count > 0)
                await _TagRepository.AddProductTagsAsync(productId, toAddTagsId);
            if (toDeleteTagsId.Count > 0)
                await _TagRepository.DeleteProductTagsAsync(productId, toDeleteTagsId);
            return true;
        }
    }
}

﻿using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface ITagRepository
    {
        Task<bool> CreateTagAsync(Tag productTag);
        Task<List<Tag>> GetTagsAsync();
        Task<List<Tag>> GetProductTagsByIdAsync(string id);
        Task<List<string>> GetTagsIdByTagsNameAsync(List<string> tagNames);
        Task<bool> UpdateTagAsync(Tag productTag);
        Task<bool> DeleteTagAsync(string name);
        Task<bool> AddProductTagsAsync(string productId, List<string> tags);
        Task<bool> DeleteProductTagsAsync(string productId, List<string> tags);
    }
}

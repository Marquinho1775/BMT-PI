using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class TagRepository : ITagRepository
    {
        private readonly string _connectionString;

        public TagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateTagAsync(Tag productTag)
        {
            var query = "INSERT INTO Tags (Name) VALUES (@Name)";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", productTag.Name);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            var tags = new List<Tag>();
            var query = "SELECT * FROM Tags";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var tag = new Tag
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString()
                    };
                    tags.Add(tag);
                }
            }
            return tags;
        }

        public async Task<List<Tag>> GetProductTagsByIdAsync(string productId)
        {
            var productTags = new List<Tag>();
            var query = "SELECT t.Id, t.Name" +
                        " FROM ProductTags pt " +
                        "JOIN Tags t ON pt.TagId = t.Id " +
                        "WHERE ProductId = @productId";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var productTag = new Tag
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString()
                    };
                    productTags.Add(productTag);
                }
            }
            return productTags;
        }
        
        public async Task<bool> UpdateTagAsync(Tag productTag)
        {
            var query = "UPDATE Tags SET Name = @Name WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", productTag.Id);
            command.Parameters.AddWithValue("@Name", productTag.Name);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteTagAsync(string name)
        {
            var query = "DELETE FROM Tags WHERE Name = @Name";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AddProductTagsAsync(string productId, List<string> tags)
        {
            var query = "INSERT INTO ProductTags (ProductId, TagId) VALUES (@ProductId, (SELECT Id FROM Tags WHERE Name = @TagName))";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            foreach (var tag in tags)
            {
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@TagName", tag);
                await command.ExecuteNonQueryAsync();
            }
            return true;
        }
    }
}

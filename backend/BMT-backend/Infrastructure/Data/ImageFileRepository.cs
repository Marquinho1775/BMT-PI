using BMT_backend.Application.Interfaces;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class ImageFileRepository : IImageFileRepository
    {
        private readonly string _connectionString;
        public ImageFileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateProductImageAsync(string productId, string relativePath)
        {
            string query = "INSERT INTO ProductImages(ProductId, URL) VALUES(@ProductId, @URL);";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@URL", relativePath);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> UpdateProfileImageAsync(string userId, string relativePath)
        {
            string query = "UPDATE Users SET ProfilePictureURL = @URL WHERE Id = @UserId;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@URL", relativePath);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<List<string>> GetProductsImagesAsync(string productId)
        {
            List<string> images = new List<string>();
            var query = "select URL from ProductImages where ProductId = @ProductId";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {

                command.Parameters.AddWithValue("@ProductId", productId);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    images.Add(reader["URL"].ToString());
                }
            }
            return images;
        }

        public async Task<string> GetProfileImageAsync(string userId)
        {
            string image = null;
            var query = "select ProfilePictureURL from Users where Id = @UserId";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    image = reader["URL"].ToString();
                }
            }
            return image;
        }
    }
}

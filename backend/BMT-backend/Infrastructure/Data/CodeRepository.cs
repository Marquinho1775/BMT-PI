using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class CodeRepository(string connectionString)
    {
        private readonly string _connectString = connectionString;

        public async Task<string> CreateCodeAsync(string userId)
        {
            Random random = new Random();
            int code = random.Next(000001, 999999);
            string formattedCode = code.ToString("D6");
            var query = @"
                MERGE INTO Codes AS target
                USING (SELECT @Id AS Id, @Code AS Code) AS source
                ON target.Id = source.Id
                WHEN MATCHED THEN
                    UPDATE SET target.Code = source.Code
                WHEN NOT MATCHED THEN
                    INSERT (Id, Code)
                    VALUES (source.Id, source.Code);
                ";
            using (var connection = new SqlConnection(_connectString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                command.Parameters.AddWithValue("@Code", formattedCode);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            return formattedCode;
        }

        public async Task<string> GetCodeAsync(string userId)
        {
            var query = "SELECT Code FROM dbo.Codes WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                await command.Connection.OpenAsync();
                var code = await command.ExecuteScalarAsync();
                return code?.ToString();
            }
        }
    }
}
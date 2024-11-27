using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class DirectionRepository : IDirectionRepository
    {

        private readonly string _connectionString;

        public DirectionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DirectionRepository()
        {
        }

        public async Task<List<Direction>> GetDirectionsFromUserAsync(string id)
        {
            var directions = new List<Direction>();
            var query = "SELECT Id, NumDirection, OtherSigns, Coordinates FROM Directions WHERE UserId = @Id AND SoftDeleted = 0";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var direction = new Direction
                    {
                        Id = reader["Id"].ToString(),
                        NumDirection = reader["NumDirection"].ToString(),
                        OtherSigns = reader["OtherSigns"] as string,
                        Coordinates = reader["Coordinates"].ToString(),
                    };
                    directions.Add(direction);
                }
            }
            return directions;
        }
        
        public async Task<bool> CreateDirectionAsync(Direction direction)
        {
            var query = "INSERT INTO Directions (UserId, NumDirection, OtherSigns, Coordinates) VALUES (@UserId, @NumDirection, @OtherSigns, @Coordinates)";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", direction.UserId);
            command.Parameters.AddWithValue("@NumDirection", direction.NumDirection);
            command.Parameters.AddWithValue("@OtherSigns", (object)direction.OtherSigns ?? DBNull.Value);
            command.Parameters.AddWithValue("@Coordinates", direction.Coordinates);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateDirectionAsync(Direction direction)
        {
            var query = "UPDATE Directions SET NumDirection = @NumDirection, OtherSigns = @OtherSigns, Coordinates = @Coordinates WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", direction.Id);
            command.Parameters.AddWithValue("@NumDirection", direction.NumDirection);
            command.Parameters.AddWithValue("@OtherSigns", (object)direction.OtherSigns ?? DBNull.Value);
            command.Parameters.AddWithValue("@Coordinates", direction.Coordinates);
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<string> GetCoordinates(string directionId)
        {
            const string query = "SELECT Coordinates FROM Directions WHERE Id = @DirectionId";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DirectionId", directionId);
            await connection.OpenAsync();
            return (string)await command.ExecuteScalarAsync();
        }

        public async Task<bool> DeleteDirectionAsync(string directionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var isUsedQuery = "SELECT COUNT(*) FROM Orders WHERE DirectionId = @DirectionId";
                        using (var isUsedCommand = new SqlCommand(isUsedQuery, connection, transaction))
                        {
                            isUsedCommand.Parameters.AddWithValue("@DirectionId", directionId);
                            var count = (int)await isUsedCommand.ExecuteScalarAsync();

                            if (count > 0)
                            {
                                var softDeleteQuery = "UPDATE dbo.Directions SET SoftDeleted = 1 WHERE Id = @Id";
                                using (var softDeleteCommand = new SqlCommand(softDeleteQuery, connection, transaction))
                                {
                                    softDeleteCommand.Parameters.AddWithValue("@Id", directionId);
                                    var rowsAffected = await softDeleteCommand.ExecuteNonQueryAsync();
                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                var hardDeleteQuery = "DELETE FROM dbo.Directions WHERE Id = @Id";
                                using (var hardDeleteCommand = new SqlCommand(hardDeleteQuery, connection, transaction))
                                {
                                    hardDeleteCommand.Parameters.AddWithValue("@Id", directionId);
                                    var rowsAffected = await hardDeleteCommand.ExecuteNonQueryAsync();
                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        return true;
                                    }
                                }
                            }
                            transaction.Rollback();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
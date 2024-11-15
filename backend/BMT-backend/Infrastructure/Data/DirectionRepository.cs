﻿using BMT_backend.Application.Interfaces;
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

        public async Task<List<Direction>> GetDirectionsFromUserAsync(string id)
        {
            var directions = new List<Direction>();
            var query = "SELECT Id, NumDirection, OtherSigns, Coordinates FROM Directions WHERE UserId = @Id";

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
                        Coordinates = reader["Coordinates"].ToString()
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
    }
}
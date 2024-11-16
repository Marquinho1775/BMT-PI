
using BMT_backend.Presentation.Requests;
using System.Data.SqlClient;
using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Handlers;

namespace BMT_backend.Infrastructure.Data
{
    public class EntrepeneurRepository : IEntrepeneurRepository
    {
        private readonly string _connectionString;
        private readonly IEnterpriseRepository _enterpriseRepository;

        public EntrepeneurRepository(string connectionString)
        {
            _connectionString = connectionString;
            _enterpriseRepository = new EnterpriseRepository(connectionString);
        }

        public async Task<bool> CheckIfEntryInTable(string tableName, string columnName, string columnValue)
        {
            var query = "select " + columnName + " from " + tableName + " where " + columnName + " = '" + columnValue + "'";
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }

        }

        public async Task<bool> CreateEntrepreneur(Entrepreneur entrepreneur)
        {
            var query = "insert into Entrepreneurs (UserId, Identification) " +
                "values ((select Id from Users where UserName = @UserName), " +
                        "@Identification);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserName", entrepreneur.Username);
                command.Parameters.AddWithValue("@Identification", entrepreneur.Identification);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }
        }

        public async Task<bool> AddEntrepreneurToEnterprise(AddEntrepreneurToEnterpriseRequest request)
        {
            var query = "insert into Entrepreneurs_Enterprises (EntrepreneurId, EnterpriseId, Administrator) " +
                "values ((select Id from Entrepreneurs where Identification = @EntrepreneurIdentification), " +
                        "(select Id from Enterprises where IdentificationNumber = @EnterpriseIdentification), " +
                        "@IsAdmin);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EntrepreneurIdentification", request.EntrepreneurIdentification);
                command.Parameters.AddWithValue("@EnterpriseIdentification", request.EnterpriseIdentification);
                command.Parameters.AddWithValue("@IsAdmin", request.IsAdmin ? 1 : 0);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected >= 1;
            }
        }

        public async Task<List<Entrepreneur>> GetEntrepreneurs()
        {
            List<Entrepreneur> entrepreneurs = new List<Entrepreneur>();
            var query = "select e.Id, e.Identification, u.Name, u.LastName, u.UserName, u.Email, u.IsVerified " +
                        "from Entrepreneurs e " +
                        "join Users u on e.UserId = u.Id;";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        entrepreneurs.Add(new Entrepreneur
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            IsVerified = (bool)reader["IsVerified"],
                            Identification = reader["Identification"].ToString()
                        });
                    }
                }
            }
            return entrepreneurs;
        }

        public async Task<List<Enterprise>> GetEntrepreneurEnterprises(string Identification)
        {
            var enterprises = new List<Enterprise>();
            var query = "select en.Id, en.IdentificationNumber, en.Name, en.Description, en.Email, en.PhoneNumber " +
                           "from Entrepreneurs_Enterprises ee " +
                           "join Enterprises en on ee.EnterpriseId = en.Id " +
                           "where ee.EntrepreneurId = (select Id from Entrepreneurs where Identification = @Identification);";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Identification", Identification);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        enterprises.Add(new Enterprise
                        {
                            Id = reader["Id"].ToString(),
                            IdentificationNumber = reader["IdentificationNumber"].ToString(),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNumber = reader["PhoneNumber"].ToString(),
                            Administrator = _enterpriseRepository.GetEnterpriseAdministratorAsync(reader["Id"].ToString()).Result,
                        });
                    }
                }
            }
            return enterprises;
        }
        public async Task<Entrepreneur> GetEntrepreneurByUserId(string id)
        {
            var entrepreneur = new Entrepreneur();
            var query = "SELECT e.Id, e.Identification " +
                        "FROM Entrepreneurs e " +
                        "JOIN Users u ON e.UserId = u.Id " +
                        "WHERE u.Id = @id";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                // Add id parameter
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        entrepreneur.Id = reader["Id"].ToString();
                        entrepreneur.Identification = reader["Identification"].ToString();
                    }
                }
            }
            return entrepreneur;
        }
    }
}

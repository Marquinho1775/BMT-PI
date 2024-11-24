using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using System.Data;
using System.Data.SqlClient;

namespace BMT_backend.Infrastructure.Data
{
    public class EnterpriseRepository : IEnterpriseRepository
    {
        private readonly string _connectionString;
        private const string getEnterpriseStaffQuery = "select e.Id, e.Identification, " +
            "u.Name, u.LastName, u.UserName, u.Email, u.IsVerified " +
            "FROM Entrepreneurs_Enterprises ee " +
            "JOIN Entrepreneurs e ON ee.EntrepreneurId = e.Id " +
            "JOIN Users u ON e.UserId = u.Id " +
            "WHERE ee.EnterpriseId = @enterpriseId";

        public EnterpriseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public EnterpriseRepository()
        {
        }

        public async Task<bool> CreateEnterpriseAsync(Enterprise enterprise)
        {
            var query = "insert into Enterprises (" +
            "IdentificationType, IdentificationNumber, Name, Description, Email, PhoneNumber) " +
            "values (@IdentificationType, @IdentificationNumber, @Name, @Description, @Email, @PhoneNumber);";

            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@IdentificationType", enterprise.IdentificationType);
            command.Parameters.AddWithValue("@IdentificationNumber", enterprise.IdentificationNumber);
            command.Parameters.AddWithValue("@Name", enterprise.Name);
            command.Parameters.AddWithValue("@Description", enterprise.Description);
            command.Parameters.AddWithValue("@Email", enterprise.Email);
            command.Parameters.AddWithValue("@PhoneNumber", enterprise.PhoneNumber);

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected >= 1;
        }


        public async Task<List<Enterprise>> GetEnterprisesAsync()
        {
            List<Enterprise> enterprises = new List<Enterprise>();
            var query = "select * FROM Enterprises WHERE SoftDeleted = 0;";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) {
                enterprises.Add(new Enterprise
                {
                    Id = reader["Id"].ToString(),
                    IdentificationType = Convert.ToInt32(reader["IdentificationType"]),
                    IdentificationNumber = reader["IdentificationNumber"].ToString(),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                });
            }
            return enterprises;
        }

        public async Task<Enterprise> GetEnterpriseByIdAsync(string enterpriseId)
        {
            var query = "SELECT * FROM Enterprises WHERE Id = @enterpriseId AND SoftDeleted = 0";
            Enterprise enterprise = null;
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                enterprise = new Enterprise
                {
                    Id = enterpriseId,
                    IdentificationType = Convert.ToInt32(reader["IdentificationType"]),
                    IdentificationNumber = reader["IdentificationNumber"].ToString(),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                };
            }
            return enterprise;
        }

        public async Task<Entrepreneur> GetEnterpriseAdministratorAsync(string enterpriseId)
        {
            var query = getEnterpriseStaffQuery + " and ee.Administrator = 1;";
            Entrepreneur administrator = new Entrepreneur();

            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                administrator = new Entrepreneur
                {
                    Id = reader["Id"].ToString(),
                    Identification = reader["Identification"].ToString(),
                    Username = reader["Username"].ToString(),
                    Name = reader["Name"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    IsVerified = (bool)reader["IsVerified"],
                };
            }
            return administrator;
        }

        public async Task<List<Entrepreneur>> GetEnterpriseStaffAsync(string enterpriseId)
        {
            List<Entrepreneur> staff = new List<Entrepreneur>();
            var query = getEnterpriseStaffQuery;
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@enterpriseId", enterpriseId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                staff.Add( new Entrepreneur
                {
                    Id = reader["Id"].ToString(),
                    Identification = reader["Identification"].ToString(),
                    Name = reader["Name"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Username = reader["Username"].ToString(),
                    Email = reader["Email"].ToString(),
                    IsVerified = (bool)reader["IsVerified"],
                });
            }
            return staff;
        }

        public async Task<int> GetProductsQuantityAsync(string id)
        {
            var query = "SELECT COUNT(*) FROM Products p WHERE p.EnterpriseId = e.@Id AND SoftDeleted = 0) AS ProductQuantity";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            var quantity = await command.ExecuteScalarAsync();
            return Convert.ToInt32(quantity);
        }

        public async Task<List<string>> GetEnterpriseProductsIdAsync(string enterpriseId)
        {
            var query = "SELECT Id FROM Products WHERE EnterpriseId = @EnterpriseId AND SoftDeleted = 0;";

            using var _conection = new SqlConnection(_connectionString);
            using var queryCommand = new SqlCommand(query, _conection);
            queryCommand.Parameters.AddWithValue("@EnterpriseId", enterpriseId);

            await _conection.OpenAsync();
            using var reader = await queryCommand.ExecuteReaderAsync();
            List<string> productsId = new List<string>();
            while (await reader.ReadAsync())
            {
                productsId.Add(reader["Id"].ToString());
            }
            return productsId;
        }

        public async Task<string> CheckExistingEnterpriseAsync(Enterprise enterprise)
        {
            var query = "EXEC CheckEnterpriseExist @IdentificationNumber, @Name, @PhoneNumber, @Email";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdentificationNumber", enterprise.IdentificationNumber);
            command.Parameters.AddWithValue("@Name", enterprise.Name);
            command.Parameters.AddWithValue("@PhoneNumber", enterprise.PhoneNumber);
            command.Parameters.AddWithValue("@Email", enterprise.Email);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var result = reader["Result"].ToString();
                return result;
            }
            return null;
        }

        public async Task<bool> UpdateEnterpriseAsync(UpdateEnterpriseRequest updatedEnterprise, List<string> fieldsToUpdate)
        {
            var updateQuery = $"UPDATE Enterprises SET {string.Join(", ", fieldsToUpdate)} WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@Id", updatedEnterprise.Id);
            if (!string.IsNullOrEmpty(updatedEnterprise.Name))
                updateCommand.Parameters.AddWithValue("@Name", updatedEnterprise.Name);
            if (!string.IsNullOrEmpty(updatedEnterprise.Description))
                updateCommand.Parameters.AddWithValue("@Description", updatedEnterprise.Description);
            if (!string.IsNullOrEmpty(updatedEnterprise.Email))
                updateCommand.Parameters.AddWithValue("@Email", updatedEnterprise.Email);
            if (!string.IsNullOrEmpty(updatedEnterprise.PhoneNumber))
                updateCommand.Parameters.AddWithValue("@PhoneNumber", updatedEnterprise.PhoneNumber);
            await connection.OpenAsync();
            bool result = await updateCommand.ExecuteNonQueryAsync() >= 1;
            await connection.CloseAsync();
            return result;
        }

        public async Task<bool> DeleteEnterpriseAsync(string enterpriseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Verificar si hay productos en órdenes
                        var productsInOrdersQuery = @"
                        SELECT COUNT(1) 
                        FROM Order_Product op
                        JOIN Products p ON op.ProductId = p.Id
                        WHERE p.EnterpriseId = @EnterpriseId;";

                        bool productsInOrders;
                        using (var command = new SqlCommand(productsInOrdersQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EnterpriseId", enterpriseId);
                            var count = (int)await command.ExecuteScalarAsync();
                            productsInOrders = count > 0;
                        }

                        // Borrar productos (soft delete si están en órdenes)
                        string deleteProductsQuery = productsInOrders
                            ? "UPDATE Products SET SoftDeleted = 1 WHERE EnterpriseId = @EnterpriseId;"
                            : "DELETE FROM Products WHERE EnterpriseId = @EnterpriseId;";
                        using (var command = new SqlCommand(deleteProductsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EnterpriseId", enterpriseId);
                            await command.ExecuteNonQueryAsync();
                        }

                        // Borrar registros de Entrepreneurs_Enterprises (always hard delete)
                        const string deleteEntrepreneursQuery = @"
                        DELETE FROM Entrepreneurs_Enterprises 
                        WHERE EnterpriseId = @EnterpriseId;";
                        using (var command = new SqlCommand(deleteEntrepreneursQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EnterpriseId", enterpriseId);
                            await command.ExecuteNonQueryAsync();
                        }

                        // Borrar empresa (soft delete si había productos en órdenes)
                        string deleteEnterpriseQuery = productsInOrders
                            ? "UPDATE Enterprises SET SoftDeleted = 1 WHERE Id = @EnterpriseId;"
                            : "DELETE FROM Enterprises WHERE Id = @EnterpriseId;";
                        using (var command = new SqlCommand(deleteEnterpriseQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EnterpriseId", enterpriseId);
                            await command.ExecuteNonQueryAsync();
                        }

                        // Confirmar la transacción
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
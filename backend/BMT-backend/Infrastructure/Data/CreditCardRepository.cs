using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using System.Data.SqlClient;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly string _connectionString;

    public CreditCardRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> CreateCreditCardAsync(CreditCard creditCard)
    {
        var query = "INSERT INTO dbo.CreditCard (UserID, Name, Number, MagicNumber, DateVenc) VALUES (@UserID, @Name, @Number, @MagicNumber, @DateVenc)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserID", creditCard.UserID);
            command.Parameters.AddWithValue("@Name", creditCard.Name);
            command.Parameters.AddWithValue("@Number", creditCard.Number);
            command.Parameters.AddWithValue("@MagicNumber", creditCard.MagicNumber);
            command.Parameters.AddWithValue("@DateVenc", creditCard.DateVenc);
            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected >= 1;
        }
    }

    public async Task<List<CreditCard>> GetCreditCardsAsync()
    {
        List<CreditCard> creditCards = new List<CreditCard>();
        var query = "SELECT * FROM dbo.CreditCard";
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                creditCards.Add(new CreditCard
                {
                    Id = reader["Id"].ToString(),
                    UserID = reader["UserID"].ToString(),
                    Name = reader["Name"].ToString(),
                    Number = reader["Number"].ToString(),
                    MagicNumber = reader["MagicNumber"].ToString(),
                    DateVenc = reader["DateVenc"].ToString()
                });
            }
        }
        return creditCards;
    }


    public async Task<List<CreditCard>> GetCreditCardsByUserIdAsync(string userId)
    {
        List<CreditCard> creditCards = new List<CreditCard>();
        var query = "SELECT * FROM dbo.CreditCard WHERE UserID = @UserID";
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@UserID", userId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                creditCards.Add(new CreditCard
                {
                    Id = reader["Id"].ToString(),
                    UserID = reader["UserID"].ToString(),
                    Name = reader["Name"].ToString(),
                    Number = reader["Number"].ToString(),
                    MagicNumber = reader["MagicNumber"].ToString(),
                    DateVenc = reader["DateVenc"].ToString()
                });
            }

        }
        return creditCards;
    }
}

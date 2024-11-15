using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly string _connectionString;

    public CreditCardRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> CreateCreditCardAsync(CreditCard creditCard)
    {
        try
        {
            var query = "INSERT INTO dbo.CreditCard (UserID, Name, Number, MagicNumber, DateVenc) VALUES (@UserID, @Name, @Number, @MagicNumber, @DateVenc)";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", creditCard.UserID);
                    command.Parameters.AddWithValue("@Name", creditCard.Name);
                    command.Parameters.AddWithValue("@Number", creditCard.Number);
                    command.Parameters.AddWithValue("@MagicNumber", creditCard.MagicNumber);
                    command.Parameters.AddWithValue("@DateVenc", creditCard.DateVenc);

                    return await command.ExecuteNonQueryAsync() >= 1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateCreditCard: {ex.Message}");
            return false;
        }
    }

    public async Task<List<CreditCard>> GetCreditCardsAsync()
    {
        List<CreditCard> creditCards = new List<CreditCard>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.CreditCard", connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            creditCards.Add(new CreditCard
                            {
                                Id = reader.GetString(reader.GetOrdinal("Id")),
                                UserID = reader.GetString(reader.GetOrdinal("UserID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Number = reader.GetString(reader.GetOrdinal("Number")),
                                MagicNumber = reader.GetString(reader.GetOrdinal("MagicNumber")),
                                DateVenc = reader.GetString(reader.GetOrdinal("DateVenc"))
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCreditCardsAsync: {ex.Message}");
        }

        return creditCards;
    }
    public async Task<List<CreditCard>> GetCreditCardsByUserIdAsync(string userId)
    {
        List<CreditCard> creditCards = new List<CreditCard>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("SELECT * FROM dbo.CreditCard WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            creditCards.Add(new CreditCard
                            {
                                Id = reader.GetString(reader.GetOrdinal("Id")),
                                UserID = reader.GetString(reader.GetOrdinal("UserID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Number = reader.GetString(reader.GetOrdinal("Number")),
                                MagicNumber = reader.GetString(reader.GetOrdinal("MagicNumber")),
                                DateVenc = reader.GetString(reader.GetOrdinal("DateVenc"))
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCreditCardsByUserIdAsync: {ex.Message}");
        }

        return creditCards;
    }
}
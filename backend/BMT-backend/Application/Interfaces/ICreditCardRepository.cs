using BMT_backend.Domain.Entities;

namespace BMT_backend.Application.Interfaces
{
    public interface ICreditCardRepository
    {
        Task<bool> CreateCreditCardAsync(CreditCard creditCard);
        Task<List<CreditCard>> GetCreditCardsAsync();
        Task<List<CreditCard>> GetCreditCardsByUserIdAsync(string userId);
    }
}
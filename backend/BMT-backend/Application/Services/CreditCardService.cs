using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;

public class CreditCardService : ICreditCardService
{
    private readonly ICreditCardRepository _creditCardRepository;

    public CreditCardService(ICreditCardRepository creditCardRepository)
    {
        _creditCardRepository = creditCardRepository;

    }

    public async Task<bool> CreateCreditCardAsync(CreditCard creditCard)
    {
        return await _creditCardRepository.CreateCreditCardAsync(creditCard);
    }

    public async Task<List<CreditCard>> GetCreditCardsAsync()
    {
        return await _creditCardRepository.GetCreditCardsAsync();
    }

    public async Task<List<CreditCard>> GetCreditCardsByUserIdAsync(string userId)
    {
        return await _creditCardRepository.GetCreditCardsByUserIdAsync(userId);
    }
}
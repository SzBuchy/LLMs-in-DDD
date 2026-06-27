using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class LoyaltyAccountService : ILoyaltyAccountService
{
    private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;

    public LoyaltyAccountService(ILoyaltyAccountRepository loyaltyAccountRepository)
    {
        _loyaltyAccountRepository = loyaltyAccountRepository;
    }

    public async Task<LoyaltyAccount> GetOrCreateAccountAsync(string buyerId)
    {
        var account = await _loyaltyAccountRepository.GetByBuyerIdAsync(buyerId);
        if (account is null)
        {
            account = new LoyaltyAccount(buyerId);
            account = await _loyaltyAccountRepository.AddAsync(account);
        }

        return account;
    }

    public async Task EarnPointsAsync(string buyerId, int points)
    {
        var account = await GetOrCreateAccountAsync(buyerId);
        account.EarnPoints(points, DateTimeOffset.UtcNow);
        await _loyaltyAccountRepository.UpdateAsync(account);
    }

    public async Task<decimal> RedeemPointsAsync(string buyerId, int points)
    {
        var account = await _loyaltyAccountRepository.GetByBuyerIdAsync(buyerId);
        if (account is null)
        {
            throw new LoyaltyAccountNotFoundException(buyerId);
        }

        var discount = account.RedeemPoints(points, DateTimeOffset.UtcNow);
        await _loyaltyAccountRepository.UpdateAsync(account);

        return discount;
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class LoyaltyAccountService : ILoyaltyAccountService
{
    private readonly IRepository<LoyaltyAccount> _loyaltyAccountRepository;

    public LoyaltyAccountService(IRepository<LoyaltyAccount> loyaltyAccountRepository)
    {
        _loyaltyAccountRepository = loyaltyAccountRepository;
    }

    public async Task<LoyaltyAccount> GetOrCreateAccountAsync(string buyerId, CancellationToken cancellationToken = default)
    {
        var account = await GetAccountAsync(buyerId, cancellationToken);
        if (account is not null)
        {
            return account;
        }

        return await _loyaltyAccountRepository.AddAsync(new LoyaltyAccount(buyerId), cancellationToken);
    }

    public async Task<int> AwardPointsForOrderAsync(
        string buyerId,
        int orderId,
        decimal orderTotal,
        DateTimeOffset awardedAt,
        CancellationToken cancellationToken = default)
    {
        var account = await GetOrCreateAccountAsync(buyerId, cancellationToken);
        var awardedPoints = account.AwardPointsForOrder(orderId, orderTotal, awardedAt);

        if (awardedPoints > 0)
        {
            await _loyaltyAccountRepository.UpdateAsync(account, cancellationToken);
        }

        return awardedPoints;
    }

    public async Task<LoyaltyPointRedemption> RedeemPointsAsync(
        string buyerId,
        int pointsToRedeem,
        DateTimeOffset redeemedAt,
        CancellationToken cancellationToken = default)
    {
        var account = await GetOrCreateAccountAsync(buyerId, cancellationToken);
        var redemption = account.RedeemPoints(pointsToRedeem, redeemedAt);

        await _loyaltyAccountRepository.UpdateAsync(account, cancellationToken);

        return redemption;
    }

    private async Task<LoyaltyAccount?> GetAccountAsync(string buyerId, CancellationToken cancellationToken)
    {
        var spec = new LoyaltyAccountByBuyerIdSpecification(buyerId);
        return await _loyaltyAccountRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }
}

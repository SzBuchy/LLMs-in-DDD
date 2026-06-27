using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface ILoyaltyAccountService
{
    Task<LoyaltyAccount> GetOrCreateAccountAsync(string buyerId, CancellationToken cancellationToken = default);

    Task<int> AwardPointsForOrderAsync(
        string buyerId,
        int orderId,
        decimal orderTotal,
        DateTimeOffset awardedAt,
        CancellationToken cancellationToken = default);

    Task<LoyaltyPointRedemption> RedeemPointsAsync(
        string buyerId,
        int pointsToRedeem,
        DateTimeOffset redeemedAt,
        CancellationToken cancellationToken = default);
}

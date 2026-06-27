using System;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public static class LoyaltyAccountMapper
{
    public static LoyaltyAccountDto ToDto(LoyaltyAccount account, DateTimeOffset asOf)
    {
        return new LoyaltyAccountDto
        {
            Id = account.Id,
            BuyerId = account.BuyerId,
            AvailablePoints = account.AvailablePoints(asOf),
            MaxPointsPerRedemption = LoyaltyAccount.MaxPointsPerRedemption,
            DiscountAmountPerPoint = LoyaltyAccount.DiscountAmountPerPoint,
            PointGrants = account.PointGrants
                .OrderBy(grant => grant.ExpiresAt)
                .Select(grant => new LoyaltyPointGrantDto
                {
                    Id = grant.Id,
                    SourceOrderId = grant.SourceOrderId,
                    PointsAwarded = grant.PointsAwarded,
                    PointsRemaining = grant.PointsRemaining,
                    AwardedAt = grant.AwardedAt,
                    ExpiresAt = grant.ExpiresAt
                })
                .ToList(),
            Redemptions = account.Redemptions
                .OrderByDescending(redemption => redemption.RedeemedAt)
                .Select(redemption => new LoyaltyPointRedemptionDto
                {
                    Id = redemption.Id,
                    PointsRedeemed = redemption.PointsRedeemed,
                    DiscountAmount = redemption.DiscountAmount,
                    RedeemedAt = redemption.RedeemedAt
                })
                .ToList()
        };
    }
}

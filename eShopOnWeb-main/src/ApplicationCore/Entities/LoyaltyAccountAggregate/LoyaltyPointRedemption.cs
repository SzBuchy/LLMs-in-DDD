using System;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyPointRedemption : BaseEntity
{
    public int LoyaltyAccountId { get; private set; }
    public int PointsRedeemed { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public DateTimeOffset RedeemedAt { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyPointRedemption() { }

    public LoyaltyPointRedemption(int pointsRedeemed, decimal discountAmount, DateTimeOffset redeemedAt)
    {
        Guard.Against.NegativeOrZero(pointsRedeemed, nameof(pointsRedeemed));
        Guard.Against.NegativeOrZero(discountAmount, nameof(discountAmount));

        PointsRedeemed = pointsRedeemed;
        DiscountAmount = discountAmount;
        RedeemedAt = redeemedAt;
    }
}

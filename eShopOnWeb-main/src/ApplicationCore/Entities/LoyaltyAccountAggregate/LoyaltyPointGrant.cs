using System;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyPointGrant : BaseEntity
{
    public int LoyaltyAccountId { get; private set; }
    public int? SourceOrderId { get; private set; }
    public int PointsAwarded { get; private set; }
    public int PointsRemaining { get; private set; }
    public DateTimeOffset AwardedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyPointGrant() { }

    public LoyaltyPointGrant(int pointsAwarded, DateTimeOffset awardedAt, DateTimeOffset expiresAt, int? sourceOrderId = null)
    {
        Guard.Against.NegativeOrZero(pointsAwarded, nameof(pointsAwarded));
        if (expiresAt <= awardedAt)
        {
            throw new ArgumentException("Points must expire after they are awarded.", nameof(expiresAt));
        }

        PointsAwarded = pointsAwarded;
        PointsRemaining = pointsAwarded;
        AwardedAt = awardedAt;
        ExpiresAt = expiresAt;
        SourceOrderId = sourceOrderId;
    }

    public bool IsExpired(DateTimeOffset asOf) => ExpiresAt <= asOf;

    public int Redeem(int requestedPoints)
    {
        Guard.Against.NegativeOrZero(requestedPoints, nameof(requestedPoints));

        var redeemedPoints = Math.Min(requestedPoints, PointsRemaining);
        PointsRemaining -= redeemedPoints;
        return redeemedPoints;
    }

    public void Expire()
    {
        PointsRemaining = 0;
    }
}

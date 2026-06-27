using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyAccount : BaseEntity, IAggregateRoot
{
    public const int PointsExpireAfterYears = 1;
    public const int MaxPointsPerRedemption = 500;
    public const decimal DiscountAmountPerPoint = 0.01m;

    public string BuyerId { get; private set; }

    private readonly List<LoyaltyPointGrant> _pointGrants = new();
    public IReadOnlyCollection<LoyaltyPointGrant> PointGrants => _pointGrants.AsReadOnly();

    private readonly List<LoyaltyPointRedemption> _redemptions = new();
    public IReadOnlyCollection<LoyaltyPointRedemption> Redemptions => _redemptions.AsReadOnly();

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyAccount() { }

    public LoyaltyAccount(string buyerId)
    {
        Guard.Against.NullOrWhiteSpace(buyerId, nameof(buyerId));

        BuyerId = buyerId;
    }

    public int AvailablePoints(DateTimeOffset asOf)
    {
        return _pointGrants
            .Where(grant => !grant.IsExpired(asOf))
            .Sum(grant => grant.PointsRemaining);
    }

    public int AwardPointsForOrder(int orderId, decimal orderTotal, DateTimeOffset awardedAt)
    {
        Guard.Against.NegativeOrZero(orderId, nameof(orderId));
        Guard.Against.NegativeOrZero(orderTotal, nameof(orderTotal));

        if (_pointGrants.Any(grant => grant.SourceOrderId == orderId))
        {
            return 0;
        }

        var points = CalculatePointsForOrder(orderTotal);
        if (points == 0)
        {
            return 0;
        }

        _pointGrants.Add(new LoyaltyPointGrant(
            points,
            awardedAt,
            awardedAt.AddYears(PointsExpireAfterYears),
            orderId));

        return points;
    }

    public LoyaltyPointRedemption RedeemPoints(int pointsToRedeem, DateTimeOffset redeemedAt)
    {
        Guard.Against.NegativeOrZero(pointsToRedeem, nameof(pointsToRedeem));
        if (pointsToRedeem > MaxPointsPerRedemption)
        {
            throw new LoyaltyAccountOperationException(
                $"Cannot redeem more than {MaxPointsPerRedemption} loyalty points at once.");
        }

        ExpirePoints(redeemedAt);

        if (AvailablePoints(redeemedAt) < pointsToRedeem)
        {
            throw new LoyaltyAccountOperationException("The loyalty account does not have enough active points.");
        }

        var remainingToRedeem = pointsToRedeem;
        foreach (var grant in _pointGrants
                     .Where(grant => grant.PointsRemaining > 0 && !grant.IsExpired(redeemedAt))
                     .OrderBy(grant => grant.ExpiresAt)
                     .ThenBy(grant => grant.AwardedAt))
        {
            remainingToRedeem -= grant.Redeem(remainingToRedeem);
            if (remainingToRedeem == 0)
            {
                break;
            }
        }

        var redemption = new LoyaltyPointRedemption(
            pointsToRedeem,
            CalculateDiscount(pointsToRedeem),
            redeemedAt);

        _redemptions.Add(redemption);
        return redemption;
    }

    public void ExpirePoints(DateTimeOffset asOf)
    {
        foreach (var grant in _pointGrants.Where(grant => grant.PointsRemaining > 0 && grant.IsExpired(asOf)))
        {
            grant.Expire();
        }
    }

    public static int CalculatePointsForOrder(decimal orderTotal)
    {
        Guard.Against.Negative(orderTotal, nameof(orderTotal));

        return (int)Math.Floor(orderTotal);
    }

    public static decimal CalculateDiscount(int points)
    {
        Guard.Against.NegativeOrZero(points, nameof(points));

        return points * DiscountAmountPerPoint;
    }
}

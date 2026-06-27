using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyAccount : BaseEntity, IAggregateRoot
{
    public const int MaxPointsRedeemablePerTransaction = 500;

    // 100 points = 1.00 currency unit of discount.
    public const decimal PointToCurrencyConversionRate = 0.01m;

#pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyAccount() { }
#pragma warning restore CS8618

    public LoyaltyAccount(string buyerId)
    {
        Guard.Against.NullOrEmpty(buyerId, nameof(buyerId));
        BuyerId = buyerId;
    }

    public string BuyerId { get; private set; }

    private readonly List<LoyaltyPointsLot> _pointsLots = new();
    public IReadOnlyCollection<LoyaltyPointsLot> PointsLots => _pointsLots.AsReadOnly();

    public void EarnPoints(int points, DateTimeOffset earnedDate)
    {
        Guard.Against.NegativeOrZero(points, nameof(points));
        _pointsLots.Add(new LoyaltyPointsLot(points, earnedDate));
    }

    public int GetAvailablePoints(DateTimeOffset asOf) =>
        _pointsLots.Sum(lot => lot.AvailablePoints(asOf));

    // Redeems points for a discount on the next order. Consumes the oldest non-expired
    // lots first, so points that are closer to expiring are used up before newer ones.
    public decimal RedeemPoints(int pointsToRedeem, DateTimeOffset asOf)
    {
        Guard.Against.NegativeOrZero(pointsToRedeem, nameof(pointsToRedeem));

        if (pointsToRedeem > MaxPointsRedeemablePerTransaction)
        {
            throw new LoyaltyPointsRedemptionLimitExceededException(pointsToRedeem, MaxPointsRedeemablePerTransaction);
        }

        var availablePoints = GetAvailablePoints(asOf);
        if (pointsToRedeem > availablePoints)
        {
            throw new InsufficientLoyaltyPointsException(BuyerId, pointsToRedeem, availablePoints);
        }

        var remainingToConsume = pointsToRedeem;
        foreach (var lot in _pointsLots
                     .Where(lot => !lot.IsExpired(asOf) && lot.RemainingPoints > 0)
                     .OrderBy(lot => lot.EarnedDate))
        {
            if (remainingToConsume == 0)
            {
                break;
            }

            var consumedFromLot = Math.Min(lot.RemainingPoints, remainingToConsume);
            lot.Consume(consumedFromLot);
            remainingToConsume -= consumedFromLot;
        }

        return pointsToRedeem * PointToCurrencyConversionRate;
    }
}

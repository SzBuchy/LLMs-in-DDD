using System;
using Ardalis.GuardClauses;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

// A single batch of points granted at one point in time (e.g. for one order).
// Tracked separately because each batch expires independently, one year after it was earned.
public class LoyaltyPointsLot : BaseEntity
{
    public static readonly TimeSpan ValidityPeriod = TimeSpan.FromDays(365);

#pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyPointsLot() { }
#pragma warning restore CS8618

    public LoyaltyPointsLot(int points, DateTimeOffset earnedDate)
    {
        Guard.Against.NegativeOrZero(points, nameof(points));

        Points = points;
        RemainingPoints = points;
        EarnedDate = earnedDate;
        ExpirationDate = earnedDate.Add(ValidityPeriod);
    }

    public int Points { get; private set; }
    public int RemainingPoints { get; private set; }
    public DateTimeOffset EarnedDate { get; private set; }
    public DateTimeOffset ExpirationDate { get; private set; }

    public bool IsExpired(DateTimeOffset asOf) => asOf > ExpirationDate;

    public int AvailablePoints(DateTimeOffset asOf) => IsExpired(asOf) ? 0 : RemainingPoints;

    internal void Consume(int points)
    {
        Guard.Against.NegativeOrZero(points, nameof(points));
        if (points > RemainingPoints)
        {
            throw new InvalidOperationException(
                $"Cannot consume {points} points from a lot that only has {RemainingPoints} remaining.");
        }

        RemainingPoints -= points;
    }
}

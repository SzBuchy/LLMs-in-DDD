using System;
using Ardalis.GuardClauses;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyPointsEntry : BaseEntity
{
    public int LoyaltyAccountId { get; private set; }
    public int Quantity { get; private set; }
    public int SpentQuantity { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }
    public string? OrderId { get; private set; }

    public int AvailableQuantity => Quantity - SpentQuantity;

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyPointsEntry() {}

    public LoyaltyPointsEntry(int quantity, DateTimeOffset createdDate, string? orderId = null)
    {
        Guard.Against.OutOfRange(quantity, nameof(quantity), 1, int.MaxValue);

        Quantity = quantity;
        SpentQuantity = 0;
        CreatedDate = createdDate;
        OrderId = orderId;
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now >= CreatedDate.AddYears(1);
    }

    public void Spend(int amount)
    {
        Guard.Against.OutOfRange(amount, nameof(amount), 1, AvailableQuantity);
        SpentQuantity += amount;
    }
}

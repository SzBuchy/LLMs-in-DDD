using System;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyTransaction : BaseEntity
{
    public int LoyaltyAccountId { get; private set; }
    public int Amount { get; private set; }
    public LoyaltyTransactionType Type { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }
    public string? OrderId { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyTransaction() {}

    public LoyaltyTransaction(int amount, LoyaltyTransactionType type, DateTimeOffset createdDate, string? orderId = null)
    {
        Amount = amount;
        Type = type;
        CreatedDate = createdDate;
        OrderId = orderId;
    }
}

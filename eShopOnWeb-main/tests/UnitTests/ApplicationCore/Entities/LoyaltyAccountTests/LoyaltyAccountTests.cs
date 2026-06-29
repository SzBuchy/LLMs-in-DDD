using System;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Entities.LoyaltyAccountTests;

public class LoyaltyAccountTests
{
    private readonly string _validCustomerId = "customer-123";

    [Fact]
    public void CreatesAccountWithCustomerId()
    {
        var account = new LoyaltyAccount(_validCustomerId);

        Assert.Equal(_validCustomerId, account.CustomerId);
        Assert.Empty(account.Entries);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void AwardPointsAddsEntryAndTransaction()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(100, now, "order-001");

        Assert.Single(account.Entries);
        Assert.Single(account.Transactions);

        var entry = account.Entries.First();
        Assert.Equal(100, entry.Quantity);
        Assert.Equal(0, entry.SpentQuantity);
        Assert.Equal(100, entry.AvailableQuantity);
        Assert.Equal(now, entry.CreatedDate);
        Assert.Equal("order-001", entry.OrderId);

        var transaction = account.Transactions.First();
        Assert.Equal(100, transaction.Amount);
        Assert.Equal(LoyaltyTransactionType.Earned, transaction.Type);
        Assert.Equal(now, transaction.CreatedDate);
        Assert.Equal("order-001", transaction.OrderId);
    }

    [Fact]
    public void GetActivePointsBalanceExcludesExpiredPoints()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(100, now.AddMonths(-13)); // Expired
        account.AwardPoints(200, now.AddMonths(-6));   // Active
        account.AwardPoints(50, now);                  // Active

        Assert.Equal(250, account.GetActivePointsBalance(now));
    }

    [Fact]
    public void SpendPointsUsesFIFOForActivePoints()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(100, now.AddMonths(-10)); // Earliest active
        account.AwardPoints(200, now.AddMonths(-6));  // Later active
        account.AwardPoints(50, now);                 // Latest active

        // Spend 150 points
        account.SpendPoints(150, now, "order-spend");

        Assert.Equal(200, account.GetActivePointsBalance(now)); // 350 - 150 = 200

        var entries = account.Entries.ToList();
        
        // First entry (100 pts) should be fully spent
        Assert.Equal(100, entries[0].SpentQuantity);
        Assert.Equal(0, entries[0].AvailableQuantity);

        // Second entry (200 pts) should be partially spent (50 pts spent, 150 left)
        Assert.Equal(50, entries[1].SpentQuantity);
        Assert.Equal(150, entries[1].AvailableQuantity);

        // Third entry (50 pts) should be untouched
        Assert.Equal(0, entries[2].SpentQuantity);
        Assert.Equal(50, entries[2].AvailableQuantity);

        // Verify transaction is recorded
        Assert.Equal(4, account.Transactions.Count); // 3 earn + 1 spend
        var spendTransaction = account.Transactions.Last();
        Assert.Equal(-150, spendTransaction.Amount);
        Assert.Equal(LoyaltyTransactionType.Spent, spendTransaction.Type);
        Assert.Equal("order-spend", spendTransaction.OrderId);
    }

    [Fact]
    public void SpendPointsThrowsIfSpendExceedsMaxPointsPerUsage()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(600, now);

        Assert.Throws<ArgumentOutOfRangeException>(() => 
            account.SpendPoints(501, now, "order-spend"));
    }

    [Fact]
    public void SpendPointsThrowsIfBalanceIsInsufficient()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(100, now);

        Assert.Throws<InvalidOperationException>(() => 
            account.SpendPoints(150, now, "order-spend"));
    }

    [Fact]
    public void SpendPointsIgnoresExpiredPoints()
    {
        var account = new LoyaltyAccount(_validCustomerId);
        var now = DateTimeOffset.Now;

        account.AwardPoints(100, now.AddMonths(-13)); // Expired
        account.AwardPoints(100, now);                  // Active

        // Attempt to spend 150 points - should fail because only 100 are active
        Assert.Throws<InvalidOperationException>(() => 
            account.SpendPoints(150, now, "order-spend"));

        // Spending 50 points should succeed and consume ONLY the active entry
        account.SpendPoints(50, now, "order-spend");

        var entries = account.Entries.ToList();
        Assert.Equal(0, entries[0].SpentQuantity); // Expired entry untouched
        Assert.Equal(50, entries[1].SpentQuantity); // Active entry consumed
    }

    [Fact]
    public void GetDiscountValueCalculatesCorrectly()
    {
        var account = new LoyaltyAccount(_validCustomerId);

        var discount = account.GetDiscountValue(100);

        Assert.Equal(10.00m, discount);
    }
}

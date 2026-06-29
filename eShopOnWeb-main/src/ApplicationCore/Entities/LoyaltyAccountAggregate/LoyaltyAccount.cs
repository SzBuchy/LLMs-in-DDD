using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

public class LoyaltyAccount : BaseEntity, IAggregateRoot
{
    public string CustomerId { get; private set; }

    private readonly List<LoyaltyPointsEntry> _entries = new List<LoyaltyPointsEntry>();
    public IReadOnlyCollection<LoyaltyPointsEntry> Entries => _entries.AsReadOnly();

    private readonly List<LoyaltyTransaction> _transactions = new List<LoyaltyTransaction>();
    public IReadOnlyCollection<LoyaltyTransaction> Transactions => _transactions.AsReadOnly();

    public const int MaxPointsPerUsage = 500;
    public const decimal PointsToDiscountConversionRate = 0.10m; // 1 point = 0.10 of currency unit

    #pragma warning disable CS8618 // Required by Entity Framework
    private LoyaltyAccount() {}

    public LoyaltyAccount(string customerId) : this()
    {
        Guard.Against.NullOrEmpty(customerId, nameof(customerId));
        CustomerId = customerId;
    }

    public int GetActivePointsBalance(DateTimeOffset now)
    {
        return _entries
            .Where(e => !e.IsExpired(now))
            .Sum(e => e.AvailableQuantity);
    }

    public decimal GetDiscountValue(int points)
    {
        return points * PointsToDiscountConversionRate;
    }

    public void AwardPoints(int points, DateTimeOffset now, string? orderId = null)
    {
        Guard.Against.OutOfRange(points, nameof(points), 1, int.MaxValue);

        var entry = new LoyaltyPointsEntry(points, now, orderId);
        _entries.Add(entry);

        var transaction = new LoyaltyTransaction(points, LoyaltyTransactionType.Earned, now, orderId);
        _transactions.Add(transaction);
    }

    public void SpendPoints(int points, DateTimeOffset now, string? orderId = null)
    {
        Guard.Against.OutOfRange(points, nameof(points), 1, MaxPointsPerUsage);

        int available = GetActivePointsBalance(now);
        if (points > available)
        {
            throw new InvalidOperationException("Niewystarczająca liczba aktywnych punktów lojalnościowych.");
        }

        int remaining = points;
        var activeEntries = _entries
            .Where(e => !e.IsExpired(now) && e.AvailableQuantity > 0)
            .OrderBy(e => e.CreatedDate)
            .ToList();

        foreach (var entry in activeEntries)
        {
            if (remaining <= 0) break;
            int spend = Math.Min(entry.AvailableQuantity, remaining);
            entry.Spend(spend);
            remaining -= spend;
        }

        var transaction = new LoyaltyTransaction(-points, LoyaltyTransactionType.Spent, now, orderId);
        _transactions.Add(transaction);
    }
}

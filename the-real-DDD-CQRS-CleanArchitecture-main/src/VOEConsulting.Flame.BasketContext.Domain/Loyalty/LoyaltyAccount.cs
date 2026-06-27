using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events;
using VOEConsulting.Flame.Common.Domain.Exceptions;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty
{
    public sealed class LoyaltyAccount : AggregateRoot<LoyaltyAccount>
    {
        // Business rule: at most this many points can be redeemed in a single transaction.
        public const int MaxPointsRedeemablePerTransaction = 500;

        // Business rule: conversion rate applied when points are exchanged for a discount.
        public const decimal PointsToDiscountConversionRate = 0.01m;

        private readonly List<LoyaltyPointsLot> _pointsLots = [];

        private LoyaltyAccount(Id<LoyaltyAccount> id, Id<Customer> customerId)
            : base(id)
        {
            CustomerId = customerId.EnsureNonNull();
        }

        public Id<Customer> CustomerId { get; }
        public IReadOnlyCollection<LoyaltyPointsLot> PointsLots => _pointsLots.AsReadOnly();

        public static LoyaltyAccount Create(Id<Customer> customerId, Id<LoyaltyAccount>? id = null)
        {
            var account = new LoyaltyAccount(id ?? Id<LoyaltyAccount>.New(), customerId);
            account.RaiseDomainEvent(new LoyaltyAccountCreatedEvent(account.Id));

            return account;
        }

        // Used by the repository to reconstruct an account from persistence, preserving
        // each lot's original earn date - business methods always stamp "now", so they
        // cannot be reused here without corrupting the expiry calculation.
        public static LoyaltyAccount Rehydrate(Id<LoyaltyAccount> id, Id<Customer> customerId, IEnumerable<LoyaltyPointsLot> pointsLots)
        {
            var account = new LoyaltyAccount(id, customerId);
            account._pointsLots.AddRange(pointsLots);

            return account;
        }

        public int GetAvailablePoints(IDateTimeProvider dateTimeProvider)
        {
            var utcNow = dateTimeProvider.UtcNow();
            return _pointsLots.Where(lot => !lot.IsExpired(utcNow)).Sum(lot => lot.Points);
        }

        public void EarnPoints(int points, IDateTimeProvider dateTimeProvider)
        {
            var lot = LoyaltyPointsLot.Create(points, dateTimeProvider.UtcNow());
            _pointsLots.Add(lot);

            RaiseDomainEvent(new LoyaltyPointsEarnedEvent(this.Id, points));
        }

        public decimal RedeemPoints(int points, IDateTimeProvider dateTimeProvider)
        {
            points.EnsureWithinRange(1, MaxPointsRedeemablePerTransaction);

            ExpirePoints(dateTimeProvider);

            var availablePoints = GetAvailablePoints(dateTimeProvider);
            if (points > availablePoints)
                throw new ValidationException($"Insufficient loyalty points balance: requested {points}, available {availablePoints}.");

            DeductPoints(points);

            var discountAmount = points * PointsToDiscountConversionRate;
            RaiseDomainEvent(new LoyaltyPointsRedeemedEvent(this.Id, points, discountAmount));

            return discountAmount;
        }

        public void ExpirePoints(IDateTimeProvider dateTimeProvider)
        {
            var utcNow = dateTimeProvider.UtcNow();
            var expiredLots = _pointsLots.Where(lot => lot.IsExpired(utcNow)).ToList();

            if (expiredLots.Count == 0)
                return;

            var expiredPoints = expiredLots.Sum(lot => lot.Points);
            _pointsLots.RemoveAll(lot => lot.IsExpired(utcNow));

            RaiseDomainEvent(new LoyaltyPointsExpiredEvent(this.Id, expiredPoints));
        }

        private void DeductPoints(int points)
        {
            var remainingToDeduct = points;
            var updatedLots = new List<LoyaltyPointsLot>();

            // Oldest lots are consumed first (FIFO), since they're closest to expiring.
            foreach (var lot in _pointsLots.OrderBy(l => l.EarnedAtUtc))
            {
                if (remainingToDeduct <= 0)
                {
                    updatedLots.Add(lot);
                }
                else if (lot.Points <= remainingToDeduct)
                {
                    remainingToDeduct -= lot.Points;
                }
                else
                {
                    updatedLots.Add(lot.Reduce(remainingToDeduct));
                    remainingToDeduct = 0;
                }
            }

            _pointsLots.Clear();
            _pointsLots.AddRange(updatedLots);
        }
    }
}

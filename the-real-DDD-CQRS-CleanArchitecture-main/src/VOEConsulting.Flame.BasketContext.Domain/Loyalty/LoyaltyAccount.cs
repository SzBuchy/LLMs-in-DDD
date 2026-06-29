using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Extensions;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty
{
    public sealed class LoyaltyAccount : AggregateRoot<LoyaltyAccount>
    {
        public Id<Customer> CustomerId { get; }
        
        private readonly List<LoyaltyPointsEntry> _pointsEntries = new();
        public IReadOnlyCollection<LoyaltyPointsEntry> PointsEntries => _pointsEntries.AsReadOnly();

        public const int MaxPointsPerRedemption = 500;

        private LoyaltyAccount(Id<Customer> customerId)
        {
            CustomerId = customerId.EnsureNonNull();
        }

        private LoyaltyAccount(Id<LoyaltyAccount> id, Id<Customer> customerId, List<LoyaltyPointsEntry> pointsEntries)
            : base(id)
        {
            CustomerId = customerId.EnsureNonNull();
            _pointsEntries = pointsEntries ?? new List<LoyaltyPointsEntry>();
        }

        public static LoyaltyAccount Create(Id<Customer> customerId)
        {
            var account = new LoyaltyAccount(customerId);
            account.RaiseDomainEvent(new LoyaltyAccountCreatedEvent(account.Id, customerId));
            return account;
        }

        public static LoyaltyAccount Reconstitute(Id<LoyaltyAccount> id, Id<Customer> customerId, List<LoyaltyPointsEntry> pointsEntries)
        {
            return new LoyaltyAccount(id, customerId, pointsEntries);
        }

        public void AddPoints(int amount, DateTimeOffset earnedAtUtc)
        {
            amount.EnsurePositive();
            
            var entry = LoyaltyPointsEntry.Create(amount, earnedAtUtc);
            _pointsEntries.Add(entry);
            
            RaiseDomainEvent(new PointsAddedEvent(Id, CustomerId, amount, earnedAtUtc));
        }

        public int GetAvailablePointsBalance(DateTimeOffset nowUtc)
        {
            return _pointsEntries.Sum(e => e.GetAvailablePoints(nowUtc));
        }

        public void RedeemPoints(int pointsToRedeem, DateTimeOffset nowUtc)
        {
            pointsToRedeem.EnsurePositive();

            if (pointsToRedeem > MaxPointsPerRedemption)
            {
                throw new ValidationException($"Cannot redeem more than {MaxPointsPerRedemption} points at once.");
            }

            var availableBalance = GetAvailablePointsBalance(nowUtc);
            if (pointsToRedeem > availableBalance)
            {
                throw new ValidationException($"Cannot redeem {pointsToRedeem} points. Only {availableBalance} points are available.");
            }

            // FIFO: oldest non-expired points first
            var activeEntries = _pointsEntries
                .Where(e => e.GetAvailablePoints(nowUtc) > 0)
                .OrderBy(e => e.EarnedAtUtc)
                .ToList();

            int remainingToRedeem = pointsToRedeem;
            foreach (var entry in activeEntries)
            {
                int availableInEntry = entry.GetAvailablePoints(nowUtc);
                int toConsume = Math.Min(remainingToRedeem, availableInEntry);

                entry.UsePoints(toConsume);
                remainingToRedeem -= toConsume;

                if (remainingToRedeem == 0)
                {
                    break;
                }
            }

            RaiseDomainEvent(new PointsRedeemedEvent(Id, CustomerId, pointsToRedeem, nowUtc));
        }
    }
}

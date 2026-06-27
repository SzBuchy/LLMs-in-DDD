using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts.Events
{
    public sealed class LoyaltyPointsExpiredEvent(
        Id<LoyaltyAccount> aggregateId,
        Id<Customer> customerId,
        int points)
        : BaseLoyaltyAccountDomainEvent(aggregateId)
    {
        public Id<Customer> CustomerId { get; } = customerId;
        public int Points { get; } = points;
    }
}

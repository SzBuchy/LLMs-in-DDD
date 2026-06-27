using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts.Events
{
    public sealed class LoyaltyAccountCreatedEvent(Id<LoyaltyAccount> aggregateId, Id<Customer> customerId)
        : BaseLoyaltyAccountDomainEvent(aggregateId)
    {
        public Id<Customer> CustomerId { get; } = customerId;
    }
}

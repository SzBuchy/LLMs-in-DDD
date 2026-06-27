using VOEConsulting.Flame.BasketContext.Domain.Common;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    [AggregateType(BasketEventConstants.LoyaltyAccountsAggregateTypeName)]
    public abstract class BaseLoyaltyAccountDomainEvent(Id<LoyaltyAccount> aggregateId, DateTimeOffset? occurredOnUtc = null)
        : DomainEvent(aggregateId, occurredOnUtc ?? DateTimeOffset.UtcNow) { }
}

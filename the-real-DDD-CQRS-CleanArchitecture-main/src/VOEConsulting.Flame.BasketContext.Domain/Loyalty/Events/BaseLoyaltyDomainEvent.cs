using VOEConsulting.Flame.BasketContext.Domain.Common;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    [AggregateType(BasketEventConstants.LoyaltyAccountsAggregateTypeName)]
    public abstract class BaseLoyaltyDomainEvent(Id<LoyaltyAccount> aggregateId, DateTimeOffset? occurredOnUtc = null)
        : DomainEvent(aggregateId, occurredOnUtc ?? DateTimeOffset.UtcNow) { }
}

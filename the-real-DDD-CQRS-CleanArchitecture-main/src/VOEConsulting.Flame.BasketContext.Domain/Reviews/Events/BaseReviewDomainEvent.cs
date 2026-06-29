using VOEConsulting.Flame.BasketContext.Domain.Common;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    [AggregateType(BasketEventConstants.ReviewsAggregateTypeName)]
    public abstract class BaseReviewDomainEvent(Id<Review> aggregateId, DateTimeOffset? occurredOnUtc = null)
        : DomainEvent(aggregateId, occurredOnUtc ?? DateTimeOffset.UtcNow) { }
}

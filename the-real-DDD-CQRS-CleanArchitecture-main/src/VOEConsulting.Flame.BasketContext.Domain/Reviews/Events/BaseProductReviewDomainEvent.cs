using VOEConsulting.Flame.BasketContext.Domain.Common;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    [AggregateType(BasketEventConstants.ProductReviewsAggregateTypeName)]
    public abstract class BaseProductReviewDomainEvent(Id<ProductReview> aggregateId, DateTimeOffset? occurredOnUtc = null)
        : DomainEvent(aggregateId, occurredOnUtc ?? DateTimeOffset.UtcNow) { }
}

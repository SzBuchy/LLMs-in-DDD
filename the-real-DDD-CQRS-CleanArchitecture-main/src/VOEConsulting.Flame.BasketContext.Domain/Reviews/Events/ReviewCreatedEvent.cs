using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ReviewCreatedEvent : BaseReviewDomainEvent
    {
        public ReviewCreatedEvent(Id<Review> reviewId) : base(reviewId) { }
    }
}

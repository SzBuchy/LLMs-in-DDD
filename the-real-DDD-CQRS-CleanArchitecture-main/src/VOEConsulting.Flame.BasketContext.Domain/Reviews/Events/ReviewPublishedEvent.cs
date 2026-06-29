using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ReviewPublishedEvent : BaseReviewDomainEvent
    {
        public ReviewPublishedEvent(Id<Review> reviewId) : base(reviewId) { }
    }
}

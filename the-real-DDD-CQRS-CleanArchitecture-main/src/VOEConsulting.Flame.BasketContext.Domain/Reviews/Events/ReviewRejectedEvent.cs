using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ReviewRejectedEvent : BaseReviewDomainEvent
    {
        public ReviewRejectedEvent(Id<Review> reviewId) : base(reviewId) { }
    }
}

using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ReviewWithdrawnEvent : BaseReviewDomainEvent
    {
        public ReviewWithdrawnEvent(Id<Review> reviewId) : base(reviewId) { }
    }
}

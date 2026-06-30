using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ReviewEditedEvent : BaseReviewDomainEvent
    {
        public ReviewEditedEvent(Id<Review> reviewId) : base(reviewId) { }
    }
}

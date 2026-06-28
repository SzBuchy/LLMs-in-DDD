namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ProductReviewEditedEvent : BaseProductReviewDomainEvent
    {
        public ProductReviewEditedEvent(Id<ProductReview> reviewId) : base(reviewId) { }
    }
}

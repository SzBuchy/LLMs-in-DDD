namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ProductReviewPublishedEvent : BaseProductReviewDomainEvent
    {
        public ProductReviewPublishedEvent(Id<ProductReview> reviewId) : base(reviewId) { }
    }
}

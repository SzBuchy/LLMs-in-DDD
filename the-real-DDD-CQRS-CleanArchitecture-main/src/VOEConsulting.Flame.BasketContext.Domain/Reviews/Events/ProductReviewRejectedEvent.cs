namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ProductReviewRejectedEvent : BaseProductReviewDomainEvent
    {
        public ProductReviewRejectedEvent(Id<ProductReview> reviewId) : base(reviewId) { }
    }
}

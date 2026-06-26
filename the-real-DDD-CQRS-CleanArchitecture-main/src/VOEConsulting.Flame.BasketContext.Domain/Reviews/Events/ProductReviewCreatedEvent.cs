namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ProductReviewCreatedEvent : BaseProductReviewDomainEvent
    {
        public ProductReviewCreatedEvent(Id<ProductReview> reviewId) : base(reviewId) { }
    }
}

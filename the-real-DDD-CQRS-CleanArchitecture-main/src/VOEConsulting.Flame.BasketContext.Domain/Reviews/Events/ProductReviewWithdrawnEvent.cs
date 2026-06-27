namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Events
{
    public sealed class ProductReviewWithdrawnEvent : BaseProductReviewDomainEvent
    {
        public ProductReviewWithdrawnEvent(Id<ProductReview> reviewId) : base(reviewId) { }
    }
}

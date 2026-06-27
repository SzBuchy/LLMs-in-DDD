namespace VOEConsulting.Flame.BasketContext.Domain.ProductReviews
{
    public interface IProductReviewPublicationPolicy
    {
        void EnsureCanPublish(ProductReview review, IEnumerable<ProductReview> existingReviews);
    }
}

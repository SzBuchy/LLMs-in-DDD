namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Policies
{
    public interface IProductReviewPublicationPolicy
    {
        void EnsureCanPublish(ProductReview review, IEnumerable<ProductReview> customerReviewsForSameProduct);
    }
}

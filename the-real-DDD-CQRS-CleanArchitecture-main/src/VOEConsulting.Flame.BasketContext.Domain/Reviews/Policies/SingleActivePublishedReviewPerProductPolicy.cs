using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews.Policies
{
    public sealed class SingleActivePublishedReviewPerProductPolicy : IProductReviewPublicationPolicy
    {
        public void EnsureCanPublish(ProductReview review, IEnumerable<ProductReview> customerReviewsForSameProduct)
        {
            var hasAnotherPublishedReview = customerReviewsForSameProduct.Any(other =>
                other.Id != review.Id
                && other.CustomerId == review.CustomerId
                && other.ProductId == review.ProductId
                && other.Status == ReviewStatus.Published);

            if (hasAnotherPublishedReview)
                throw new ValidationException(
                    "Customer already has a published review for this product. Withdraw the existing review before publishing a new one.");
        }
    }
}

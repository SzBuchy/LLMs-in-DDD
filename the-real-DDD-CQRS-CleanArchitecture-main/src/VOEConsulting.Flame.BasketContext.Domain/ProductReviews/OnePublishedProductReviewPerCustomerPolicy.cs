using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.ProductReviews
{
    public sealed class OnePublishedProductReviewPerCustomerPolicy : IProductReviewPublicationPolicy
    {
        public void EnsureCanPublish(ProductReview review, IEnumerable<ProductReview> existingReviews)
        {
            review.EnsureNonNull();
            existingReviews.EnsureNonNull();

            var hasPublishedReviewForProduct = existingReviews.Any(existingReview =>
                existingReview.Id != review.Id &&
                existingReview.CustomerId == review.CustomerId &&
                existingReview.ProductId == review.ProductId &&
                existingReview.Status == ProductReviewStatus.Published);

            if (hasPublishedReviewForProduct)
            {
                throw new ValidationException("Customer already has a published review for this product.");
            }
        }
    }
}

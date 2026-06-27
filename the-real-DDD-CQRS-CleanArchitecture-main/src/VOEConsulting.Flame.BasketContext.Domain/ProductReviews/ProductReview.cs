using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Domain.ProductReviews
{
    public sealed class ProductReview : AggregateRoot<ProductReview>
    {
        public const int MinRating = 1;
        public const int MaxRating = 5;
        public const int MinContentLength = 10;
        public const int MaxContentLength = 500;

        private ProductReview(Id<Customer> customerId, Guid productId, int rating, string content)
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNotDefault();
            Rating = rating.EnsureWithinRange(MinRating, MaxRating);
            Content = content.EnsureNonBlank().EnsureLengthInRange(MinContentLength, MaxContentLength);
            Status = ProductReviewStatus.PendingModeration;
        }

        public Id<Customer> CustomerId { get; }
        public Guid ProductId { get; }
        public int Rating { get; }
        public string Content { get; }
        public ProductReviewStatus Status { get; private set; }

        public static ProductReview Create(Id<Customer> customerId, Guid productId, int rating, string content)
        {
            return new ProductReview(customerId, productId, rating, content);
        }

        public void Publish()
        {
            EnsurePendingModeration();
            Status = ProductReviewStatus.Published;
        }

        public void Reject()
        {
            EnsurePendingModeration();
            Status = ProductReviewStatus.Rejected;
        }

        private void EnsurePendingModeration()
        {
            (Status == ProductReviewStatus.PendingModeration).EnsureTrue();
        }
    }
}

using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Domain.ProductReviews
{
    public sealed class ProductReview : AggregateRoot<ProductReview>
    {
        public const int MinRating = 1;
        public const int MaxRating = 5;
        public const int MinContentLength = 10;
        public const int MaxContentLength = 500;

        private ProductReview(
            Id<Customer> customerId,
            Guid productId,
            int rating,
            string content,
            ProductReviewStatus status,
            Id<ProductReview>? id = null)
            : base(id ?? Id<ProductReview>.New())
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNotDefault();
            SetReviewDetails(rating, content);
            Status = status;
        }

        public Id<Customer> CustomerId { get; }
        public Guid ProductId { get; }
        public int Rating { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public ProductReviewStatus Status { get; private set; }

        public static ProductReview Create(Id<Customer> customerId, Guid productId, int rating, string content)
        {
            return new ProductReview(customerId, productId, rating, content, ProductReviewStatus.PendingModeration);
        }

        public static ProductReview Restore(Id<ProductReview> id, Id<Customer> customerId, Guid productId, int rating, string content, ProductReviewStatus status)
        {
            return new ProductReview(customerId, productId, rating, content, status, id);
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

        public void Edit(Id<Customer> customerId, int rating, string content)
        {
            EnsurePublished();
            (CustomerId == customerId.EnsureNonNull()).EnsureTrue();

            SetReviewDetails(rating, content);
            Status = ProductReviewStatus.PendingModeration;
        }

        private void SetReviewDetails(int rating, string content)
        {
            Rating = rating.EnsureWithinRange(MinRating, MaxRating);
            Content = content.EnsureNonBlank().EnsureLengthInRange(MinContentLength, MaxContentLength);
        }

        private void EnsurePublished()
        {
            (Status == ProductReviewStatus.Published).EnsureTrue();
        }

        private void EnsurePendingModeration()
        {
            (Status == ProductReviewStatus.PendingModeration).EnsureTrue();
        }
    }
}

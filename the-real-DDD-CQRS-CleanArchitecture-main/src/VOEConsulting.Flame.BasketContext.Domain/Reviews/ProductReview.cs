using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Policies;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews
{
    public sealed class ProductReview : AggregateRoot<ProductReview>
    {
        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int MinContentLength = 10;
        private const int MaxContentLength = 500;

        private ProductReview(Id<Customer> customerId, Guid productId, int rating, string content)
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNotDefault();
            Rating = rating.EnsureWithinRange(MinRating, MaxRating);
            Content = content.EnsureLengthInRange(MinContentLength, MaxContentLength);
            Status = ReviewStatus.PendingModeration;
        }

        public Id<Customer> CustomerId { get; }
        public Guid ProductId { get; }
        public int Rating { get; }
        public string Content { get; }
        public ReviewStatus Status { get; private set; }

        public static ProductReview Create(Id<Customer> customerId, Guid productId, int rating, string content)
        {
            var review = new ProductReview(customerId, productId, rating, content);
            review.RaiseDomainEvent(new ProductReviewCreatedEvent(review.Id));

            return review;
        }

        public void Publish(IProductReviewPublicationPolicy publicationPolicy, IEnumerable<ProductReview> customerReviewsForSameProduct)
        {
            EnsureStatus(ReviewStatus.PendingModeration);
            publicationPolicy.EnsureCanPublish(this, customerReviewsForSameProduct);

            Status = ReviewStatus.Published;

            RaiseDomainEvent(new ProductReviewPublishedEvent(this.Id));
        }

        public void Reject()
        {
            EnsureStatus(ReviewStatus.PendingModeration);
            Status = ReviewStatus.Rejected;

            RaiseDomainEvent(new ProductReviewRejectedEvent(this.Id));
        }

        public void Withdraw()
        {
            EnsureStatus(ReviewStatus.Published);
            Status = ReviewStatus.Withdrawn;

            RaiseDomainEvent(new ProductReviewWithdrawnEvent(this.Id));
        }

        private void EnsureStatus(ReviewStatus expected)
        {
            if (Status != expected)
                throw new ValidationException($"Cannot transition review from '{Status.Name}' status.");
        }
    }
}

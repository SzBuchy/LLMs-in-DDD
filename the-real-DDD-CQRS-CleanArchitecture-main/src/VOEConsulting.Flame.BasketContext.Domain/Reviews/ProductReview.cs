using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews
{
    public sealed class ProductReview : AggregateRoot<ProductReview>
    {
        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int MinContentLength = 10;
        private const int MaxContentLength = 500;

        private ProductReview(Id<ProductReview> id, Id<Customer> customerId, Guid productId, int rating, string content)
            : base(id)
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNotDefault();
            Rating = rating.EnsureWithinRange(MinRating, MaxRating);
            Content = content.EnsureLengthInRange(MinContentLength, MaxContentLength);
            Status = ReviewStatus.PendingModeration;
        }

        public Id<Customer> CustomerId { get; }
        public Guid ProductId { get; }
        public int Rating { get; private set; }
        public string Content { get; private set; }
        public ReviewStatus Status { get; private set; }

        public static ProductReview Create(Id<Customer> customerId, Guid productId, int rating, string content, Id<ProductReview>? id = null)
        {
            var review = new ProductReview(id ?? Id<ProductReview>.New(), customerId, productId, rating, content);
            review.RaiseDomainEvent(new ProductReviewCreatedEvent(review.Id));

            return review;
        }

        public void Publish()
        {
            EnsureStatus(ReviewStatus.PendingModeration);
            Status = ReviewStatus.Published;

            RaiseDomainEvent(new ProductReviewPublishedEvent(this.Id));
        }

        public void Reject()
        {
            EnsureStatus(ReviewStatus.PendingModeration);
            Status = ReviewStatus.Rejected;

            RaiseDomainEvent(new ProductReviewRejectedEvent(this.Id));
        }

        public void Edit(int rating, string content)
        {
            EnsureStatus(ReviewStatus.Published);

            Rating = rating.EnsureWithinRange(MinRating, MaxRating);
            Content = content.EnsureLengthInRange(MinContentLength, MaxContentLength);
            Status = ReviewStatus.PendingModeration;

            RaiseDomainEvent(new ProductReviewEditedEvent(this.Id));
        }

        private void EnsureStatus(ReviewStatus expected)
        {
            if (Status != expected)
                throw new ValidationException($"Cannot transition review from '{Status.Name}' status.");
        }
    }
}

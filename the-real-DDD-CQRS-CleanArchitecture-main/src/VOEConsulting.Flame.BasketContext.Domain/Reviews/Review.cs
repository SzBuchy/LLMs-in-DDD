using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;
using VOEConsulting.Flame.Common.Domain.Extensions;

namespace VOEConsulting.Flame.BasketContext.Domain.Reviews
{
    public sealed class Review : AggregateRoot<Review>
    {
        public Id<Customer> CustomerId { get; }
        public Id<Product> ProductId { get; }
        public int Rating { get; }
        public string Content { get; }
        public ReviewStatus Status { get; private set; }

        private Review(Id<Customer> customerId, Id<Product> productId, int rating, string content)
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNonNull();
            Rating = rating.EnsureWithinRange(1, 5);
            Content = content.EnsureNonNull().EnsureLengthInRange(10, 500);
            Status = ReviewStatus.PendingModeration;
        }

        private Review(Id<Review> id, Id<Customer> customerId, Id<Product> productId, int rating, string content, ReviewStatus status)
            : base(id)
        {
            CustomerId = customerId.EnsureNonNull();
            ProductId = productId.EnsureNonNull();
            Rating = rating.EnsureWithinRange(1, 5);
            Content = content.EnsureNonNull().EnsureLengthInRange(10, 500);
            Status = status;
        }

        public static Review Create(Id<Customer> customerId, Id<Product> productId, int rating, string content)
        {
            var review = new Review(customerId, productId, rating, content);
            review.RaiseDomainEvent(new ReviewCreatedEvent(review.Id));
            return review;
        }

        public static Review Reconstitute(Id<Review> id, Id<Customer> customerId, Id<Product> productId, int rating, string content, ReviewStatus status)
        {
            return new Review(id, customerId, productId, rating, content, status);
        }

        public async Task PublishAsync(IProductReviewPublicationPolicy publicationPolicy, CancellationToken cancellationToken = default)
        {
            publicationPolicy.EnsureNonNull();

            if (Status == ReviewStatus.Published)
            {
                return; // No-op, already published
            }

            // Allowed transitions: PendingModeration -> Published, Rejected -> Published
            if (Status != ReviewStatus.PendingModeration && Status != ReviewStatus.Rejected)
            {
                throw new ValidationException($"Cannot transition status from {Status} to {ReviewStatus.Published}.");
            }

            if (!await publicationPolicy.CanPublishAsync(CustomerId, ProductId, cancellationToken))
            {
                throw new ValidationException("Customer already has a published review for this product.");
            }

            Status = ReviewStatus.Published;
            RaiseDomainEvent(new ReviewPublishedEvent(this.Id));
        }

        public void Withdraw()
        {
            if (Status == ReviewStatus.Withdrawn)
            {
                return; // No-op, already withdrawn
            }

            // Allowed transitions: PendingModeration -> Withdrawn, Published -> Withdrawn
            if (Status != ReviewStatus.PendingModeration && Status != ReviewStatus.Published)
            {
                throw new ValidationException($"Cannot transition status from {Status} to {ReviewStatus.Withdrawn}.");
            }

            Status = ReviewStatus.Withdrawn;
            RaiseDomainEvent(new ReviewWithdrawnEvent(this.Id));
        }

        public void Reject()
        {
            if (Status == ReviewStatus.Rejected)
            {
                return; // No-op, already rejected
            }

            // Allowed transitions: PendingModeration -> Rejected, Published -> Rejected
            if (Status != ReviewStatus.PendingModeration && Status != ReviewStatus.Published)
            {
                throw new ValidationException($"Cannot transition status from {Status} to {ReviewStatus.Rejected}.");
            }

            Status = ReviewStatus.Rejected;
            RaiseDomainEvent(new ReviewRejectedEvent(this.Id));
        }
    }
}

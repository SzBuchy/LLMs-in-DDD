using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class ProductReviewTests
    {
        private static readonly Id<Customer> CustomerId = Id<Customer>.New();
        private static readonly Guid ProductId = Guid.NewGuid();
        private static readonly OnePublishedProductReviewPerCustomerPolicy PublicationPolicy = new();

        [Fact]
        public void Create_WhenValidArgumentsProvided_ShouldCreatePendingReview()
        {
            // Act
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");

            // Assert
            review.CustomerId.Should().Be(CustomerId);
            review.ProductId.Should().Be(ProductId);
            review.Rating.Should().Be(5);
            review.Content.Should().Be("Very good product.");
            review.Status.Should().Be(ProductReviewStatus.PendingModeration);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Create_WhenRatingIsOutsideRange_ShouldFail(int rating)
        {
            // Act
            var action = () => ProductReview.Create(CustomerId, ProductId, rating, "Very good product.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("Too short")]
        public void Create_WhenContentIsTooShortOrBlank_ShouldFail(string content)
        {
            // Act
            var action = () => ProductReview.Create(CustomerId, ProductId, 5, content);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Create_WhenContentIsTooLong_ShouldFail()
        {
            // Arrange
            var content = new string('a', ProductReview.MaxContentLength + 1);

            // Act
            var action = () => ProductReview.Create(CustomerId, ProductId, 5, content);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Create_WhenProductIdIsDefault_ShouldFail()
        {
            // Act
            var action = () => ProductReview.Create(CustomerId, Guid.Empty, 5, "Very good product.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Publish_WhenReviewIsPendingModeration_ShouldChangeStatusToPublished()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");

            // Act
            review.Publish(PublicationPolicy, []);

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Published);
        }

        [Fact]
        public void Publish_WhenCustomerAlreadyHasPublishedReviewForProduct_ShouldFail()
        {
            // Arrange
            var publishedReview = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            publishedReview.Publish(PublicationPolicy, []);
            var review = ProductReview.Create(CustomerId, ProductId, 4, "Good product too.");

            // Act
            var action = () => review.Publish(PublicationPolicy, [publishedReview]);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
            review.Status.Should().Be(ProductReviewStatus.PendingModeration);
        }

        [Fact]
        public void Publish_WhenPreviousReviewWasWithdrawn_ShouldChangeStatusToPublished()
        {
            // Arrange
            var withdrawnReview = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            withdrawnReview.Publish(PublicationPolicy, []);
            withdrawnReview.Withdraw();
            var review = ProductReview.Create(CustomerId, ProductId, 4, "Good product too.");

            // Act
            review.Publish(PublicationPolicy, [withdrawnReview]);

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Published);
        }

        [Fact]
        public void Publish_WhenPublishedReviewBelongsToDifferentProduct_ShouldChangeStatusToPublished()
        {
            // Arrange
            var publishedReview = ProductReview.Create(CustomerId, Guid.NewGuid(), 5, "Very good product.");
            publishedReview.Publish(PublicationPolicy, []);
            var review = ProductReview.Create(CustomerId, ProductId, 4, "Good product too.");

            // Act
            review.Publish(PublicationPolicy, [publishedReview]);

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Published);
        }

        [Fact]
        public void Publish_WhenPublishedReviewBelongsToDifferentCustomer_ShouldChangeStatusToPublished()
        {
            // Arrange
            var publishedReview = ProductReview.Create(Id<Customer>.New(), ProductId, 5, "Very good product.");
            publishedReview.Publish(PublicationPolicy, []);
            var review = ProductReview.Create(CustomerId, ProductId, 4, "Good product too.");

            // Act
            review.Publish(PublicationPolicy, [publishedReview]);

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Published);
        }

        [Fact]
        public void Reject_WhenReviewIsPendingModeration_ShouldChangeStatusToRejected()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");

            // Act
            review.Reject();

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Rejected);
        }

        [Fact]
        public void Reject_WhenReviewIsPublished_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish(PublicationPolicy, []);

            // Act
            var action = () => review.Reject();

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Publish_WhenReviewIsRejected_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Reject();

            // Act
            var action = () => review.Publish(PublicationPolicy, []);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Withdraw_WhenReviewIsPublished_ShouldChangeStatusToWithdrawn()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish(PublicationPolicy, []);

            // Act
            review.Withdraw();

            // Assert
            review.Status.Should().Be(ProductReviewStatus.Withdrawn);
        }

        [Fact]
        public void Withdraw_WhenReviewIsPendingModeration_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");

            // Act
            var action = () => review.Withdraw();

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }
    }
}

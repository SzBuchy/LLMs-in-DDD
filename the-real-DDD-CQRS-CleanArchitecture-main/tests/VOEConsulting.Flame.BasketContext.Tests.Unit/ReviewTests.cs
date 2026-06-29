using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Tests.Unit.Extensions;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class ReviewTests
    {
        private readonly Id<Customer> _customerId = Id<Customer>.New();
        private readonly Id<Product> _productId = Id<Product>.New();
        private const string ValidContent = "This is a valid review content with enough characters.";

        [Fact]
        public void Create_WhenValidDataProvided_ShouldInitializeCorrectlyAndRaiseReviewCreatedEvent()
        {
            // Arrange & Act
            var review = Review.Create(_customerId, _productId, 4, ValidContent);
            var expectedEvent = new ReviewCreatedEvent(review.Id);

            // Assert
            review.CustomerId.Should().Be(_customerId);
            review.ProductId.Should().Be(_productId);
            review.Rating.Should().Be(4);
            review.Content.Should().Be(ValidContent);
            review.Status.Should().Be(ReviewStatus.PendingModeration);

            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        [InlineData(100)]
        public void Create_WhenRatingIsOutOfRange_ShouldThrowValidationException(int invalidRating)
        {
            // Arrange & Act
            var action = () => Review.Create(_customerId, _productId, invalidRating, ValidContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineData("Short")]
        [InlineData("")]
        public void Create_WhenContentIsTooShort_ShouldThrowValidationException(string tooShortContent)
        {
            // Arrange & Act
            var action = () => Review.Create(_customerId, _productId, 5, tooShortContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Create_WhenContentIsTooLong_ShouldThrowValidationException()
        {
            // Arrange
            var tooLongContent = new string('a', 501);

            // Act
            var action = () => Review.Create(_customerId, _productId, 5, tooLongContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Create_WhenContentIsNull_ShouldThrowValidationException()
        {
            // Arrange & Act
            var action = () => Review.Create(_customerId, _productId, 5, null!);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Create_WhenCustomerIdIsNull_ShouldThrowValidationException()
        {
            // Arrange & Act
            var action = () => Review.Create(null!, _productId, 5, ValidContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Create_WhenProductIdIsNull_ShouldThrowValidationException()
        {
            // Arrange & Act
            var action = () => Review.Create(_customerId, null!, 5, ValidContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public void Publish_WhenStatusIsPendingModeration_ShouldSetStatusToPublishedAndRaiseReviewPublishedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.ClearEvents();

            var expectedEvent = new ReviewPublishedEvent(review.Id);

            // Act
            review.Publish();

            // Assert
            review.Status.Should().Be(ReviewStatus.Published);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Publish_WhenStatusIsRejected_ShouldSetStatusToPublishedAndRaiseReviewPublishedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Reject();
            review.ClearEvents();

            var expectedEvent = new ReviewPublishedEvent(review.Id);

            // Act
            review.Publish();

            // Assert
            review.Status.Should().Be(ReviewStatus.Published);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Publish_WhenStatusIsAlreadyPublished_ShouldBeNoOp()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Publish();
            review.ClearEvents();

            // Act
            review.Publish();

            // Assert
            review.Status.Should().Be(ReviewStatus.Published);
            review.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void Reject_WhenStatusIsPendingModeration_ShouldSetStatusToRejectedAndRaiseReviewRejectedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.ClearEvents();

            var expectedEvent = new ReviewRejectedEvent(review.Id);

            // Act
            review.Reject();

            // Assert
            review.Status.Should().Be(ReviewStatus.Rejected);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Reject_WhenStatusIsPublished_ShouldSetStatusToRejectedAndRaiseReviewRejectedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Publish();
            review.ClearEvents();

            var expectedEvent = new ReviewRejectedEvent(review.Id);

            // Act
            review.Reject();

            // Assert
            review.Status.Should().Be(ReviewStatus.Rejected);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Reject_WhenStatusIsAlreadyRejected_ShouldBeNoOp()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Reject();
            review.ClearEvents();

            // Act
            review.Reject();

            // Assert
            review.Status.Should().Be(ReviewStatus.Rejected);
            review.DomainEvents.Should().BeEmpty();
        }
    }
}

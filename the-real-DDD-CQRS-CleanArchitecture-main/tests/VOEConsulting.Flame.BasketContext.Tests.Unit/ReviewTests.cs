using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.BasketContext.Tests.Unit.Extensions;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task PublishAsync_WhenStatusIsPendingModeration_ShouldSetStatusToPublishedAndRaiseReviewPublishedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.ClearEvents();

            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var expectedEvent = new ReviewPublishedEvent(review.Id);

            // Act
            await review.PublishAsync(policy);

            // Assert
            review.Status.Should().Be(ReviewStatus.Published);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public async Task PublishAsync_WhenStatusIsRejected_ShouldSetStatusToPublishedAndRaiseReviewPublishedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Reject();
            review.ClearEvents();

            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var expectedEvent = new ReviewPublishedEvent(review.Id);

            // Act
            await review.PublishAsync(policy);

            // Assert
            review.Status.Should().Be(ReviewStatus.Published);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public async Task PublishAsync_WhenStatusIsAlreadyPublished_ShouldBeNoOp()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);
            review.ClearEvents();

            // Act
            await review.PublishAsync(policy);

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
        public async Task Reject_WhenStatusIsPublished_ShouldSetStatusToRejectedAndRaiseReviewRejectedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);
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

        [Fact]
        public async Task PublishAsync_WhenPolicyDenies_ShouldThrowValidationException()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

            // Act
            var action = () => review.PublishAsync(policy);

            // Assert
            await action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Withdraw_WhenStatusIsPublished_ShouldSetStatusToWithdrawnAndRaiseReviewWithdrawnEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);
            review.ClearEvents();

            var expectedEvent = new ReviewWithdrawnEvent(review.Id);

            // Act
            review.Withdraw();

            // Assert
            review.Status.Should().Be(ReviewStatus.Withdrawn);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Withdraw_WhenStatusIsPendingModeration_ShouldSetStatusToWithdrawnAndRaiseReviewWithdrawnEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.ClearEvents();

            var expectedEvent = new ReviewWithdrawnEvent(review.Id);

            // Act
            review.Withdraw();

            // Assert
            review.Status.Should().Be(ReviewStatus.Withdrawn);
            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Withdraw_WhenStatusIsAlreadyWithdrawn_ShouldBeNoOp()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Withdraw();
            review.ClearEvents();

            // Act
            review.Withdraw();

            // Assert
            review.Status.Should().Be(ReviewStatus.Withdrawn);
            review.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void Withdraw_WhenStatusIsRejected_ShouldThrowValidationException()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 5, ValidContent);
            review.Reject();

            // Act
            var action = () => review.Withdraw();

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Fact]
        public async Task ProductReviewPublicationPolicy_WhenNoPublishedReviewExists_ShouldReturnTrue()
        {
            // Arrange
            var repository = Substitute.For<IReviewRepository>();
            repository.HasPublishedReviewForProductAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

            var policy = new ProductReviewPublicationPolicy(repository);

            // Act
            var result = await policy.CanPublishAsync(_customerId, _productId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ProductReviewPublicationPolicy_WhenPublishedReviewExists_ShouldReturnFalse()
        {
            // Arrange
            var repository = Substitute.For<IReviewRepository>();
            repository.HasPublishedReviewForProductAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var policy = new ProductReviewPublicationPolicy(repository);

            // Act
            var result = await policy.CanPublishAsync(_customerId, _productId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Edit_WhenStatusIsPublished_ShouldUpdateRatingAndContentAndTransitionStatusToPendingModerationAndRaiseReviewEditedEvent()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 4, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);
            review.ClearEvents();

            var newContent = "This is a new valid content for edited review.";
            var expectedEvent = new ReviewEditedEvent(review.Id);

            // Act
            review.Edit(5, newContent);

            // Assert
            review.Rating.Should().Be(5);
            review.Content.Should().Be(newContent);
            review.Status.Should().Be(ReviewStatus.PendingModeration);

            var actualEvent = review.DomainEvents.Single();
            actualEvent.Should().BeEquivalentEventTo(expectedEvent);
        }

        [Fact]
        public void Edit_WhenStatusIsNotPublished_ShouldThrowValidationException()
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 4, ValidContent);

            // Act
            var action = () => review.Edit(5, "This is some new content.");

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task Edit_WhenRatingIsOutOfRange_ShouldThrowValidationException(int invalidRating)
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 4, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);

            // Act
            var action = () => review.Edit(invalidRating, "This is some new content.");

            // Assert
            action.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineData("Short")]
        [InlineData("")]
        public async Task Edit_WhenContentIsTooShort_ShouldThrowValidationException(string tooShortContent)
        {
            // Arrange
            var review = Review.Create(_customerId, _productId, 4, ValidContent);
            var policy = Substitute.For<IProductReviewPublicationPolicy>();
            policy.CanPublishAsync(_customerId, _productId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            await review.PublishAsync(policy);

            // Act
            var action = () => review.Edit(5, tooShortContent);

            // Assert
            action.Should().Throw<ValidationException>();
        }
    }
}

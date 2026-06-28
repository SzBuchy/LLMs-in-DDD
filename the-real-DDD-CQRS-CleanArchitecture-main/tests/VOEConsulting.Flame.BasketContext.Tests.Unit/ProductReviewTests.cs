using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class ProductReviewTests
    {
        private static readonly Id<Customer> CustomerId = Id<Customer>.New();
        private static readonly Guid ProductId = Guid.NewGuid();

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
            review.Publish();

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
            review.Publish();

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
            var action = () => review.Publish();

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Edit_WhenReviewIsPublishedAndCustomerOwnsReview_ShouldUpdateReviewAndRequireModeration()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish();

            // Act
            review.Edit(CustomerId, 4, "Good product after longer usage.");

            // Assert
            review.Rating.Should().Be(4);
            review.Content.Should().Be("Good product after longer usage.");
            review.Status.Should().Be(ProductReviewStatus.PendingModeration);
        }

        [Fact]
        public void Edit_WhenReviewIsPendingModeration_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");

            // Act
            var action = () => review.Edit(CustomerId, 4, "Good product after longer usage.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Edit_WhenReviewIsRejected_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Reject();

            // Act
            var action = () => review.Edit(CustomerId, 4, "Good product after longer usage.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Edit_WhenCustomerDoesNotOwnReview_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish();

            // Act
            var action = () => review.Edit(Id<Customer>.New(), 4, "Good product after longer usage.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Edit_WhenRatingIsOutsideRange_ShouldFail(int rating)
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish();

            // Act
            var action = () => review.Edit(CustomerId, rating, "Good product after longer usage.");

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }

        [Fact]
        public void Edit_WhenContentIsTooLong_ShouldFail()
        {
            // Arrange
            var review = ProductReview.Create(CustomerId, ProductId, 5, "Very good product.");
            review.Publish();
            var content = new string('a', ProductReview.MaxContentLength + 1);

            // Act
            var action = () => review.Edit(CustomerId, 4, content);

            // Assert
            action.Should().ThrowExactly<ValidationException>();
        }
    }
}

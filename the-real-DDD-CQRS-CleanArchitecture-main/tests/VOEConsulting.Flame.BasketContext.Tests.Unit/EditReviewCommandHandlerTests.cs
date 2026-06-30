using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditReview;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;
using VOEConsulting.Flame.Common.Domain.Errors;
using Xunit;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class EditReviewCommandHandlerTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly EditReviewCommandHandler _handler;

        public EditReviewCommandHandlerTests()
        {
            _reviewRepository = Substitute.For<IReviewRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _handler = new EditReviewCommandHandler(_reviewRepository, _unitOfWork, _domainEventDispatcher);
        }

        [Fact]
        public async Task Handle_WhenValidCommand_ShouldEditReviewSaveAndDispatchEvents()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var existingReview = Review.Reconstitute(
                Id<Review>.FromGuid(reviewId),
                Id<Customer>.FromGuid(customerId),
                Id<Product>.FromGuid(productId),
                4,
                "This is the original content that has enough characters.",
                ReviewStatus.Published
            );

            _reviewRepository.GetByIdAsync(existingReview.Id, Arg.Any<CancellationToken>()).Returns(existingReview);

            var command = new EditReviewCommand(reviewId, customerId, 5, "This is the newly edited review content and it is longer.");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(reviewId);

            existingReview.Rating.Should().Be(5);
            existingReview.Content.Should().Be("This is the newly edited review content and it is longer.");
            existingReview.Status.Should().Be(ReviewStatus.PendingModeration);

            await _reviewRepository.Received(1).UpdateAsync(existingReview, Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _domainEventDispatcher.Received(1).DispatchAsync(
                Arg.Is<System.Collections.Generic.IEnumerable<IDomainEvent>>(events =>
                    System.Linq.Enumerable.Any(events, e => e is ReviewEditedEvent && e.AggregateId == reviewId)),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenReviewNotFound_ShouldReturnNotFoundError()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            _reviewRepository.GetByIdAsync(Arg.Any<Id<Review>>(), Arg.Any<CancellationToken>()).Returns((Review?)null);

            var command = new EditReviewCommand(reviewId, customerId, 5, "This is the newly edited review content and it is longer.");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorType.Should().Be(ErrorType.NotFound);

            await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenCustomerNotAuthorized_ShouldReturnBadRequestError()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var otherCustomerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var existingReview = Review.Reconstitute(
                Id<Review>.FromGuid(reviewId),
                Id<Customer>.FromGuid(customerId),
                Id<Product>.FromGuid(productId),
                4,
                "This is the original content that has enough characters.",
                ReviewStatus.Published
            );

            _reviewRepository.GetByIdAsync(existingReview.Id, Arg.Any<CancellationToken>()).Returns(existingReview);

            var command = new EditReviewCommand(reviewId, otherCustomerId, 5, "This is the newly edited review content and it is longer.");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorType.Should().Be(ErrorType.BadRequest);

            await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenDomainThrowsException_ShouldReturnValidationError()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var existingReview = Review.Reconstitute(
                Id<Review>.FromGuid(reviewId),
                Id<Customer>.FromGuid(customerId),
                Id<Product>.FromGuid(productId),
                4,
                "This is the original content that has enough characters.",
                ReviewStatus.PendingModeration // Edit will fail because status is not Published
            );

            _reviewRepository.GetByIdAsync(existingReview.Id, Arg.Any<CancellationToken>()).Returns(existingReview);

            var command = new EditReviewCommand(reviewId, customerId, 5, "This is the newly edited review content and it is longer.");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.ErrorType.Should().Be(ErrorType.Validation);

            await _reviewRepository.DidNotReceive().UpdateAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}

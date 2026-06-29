using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Events;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Events;
using Xunit;

namespace VOEConsulting.Flame.BasketContext.Tests.Unit
{
    public class AddReviewCommandHandlerTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly AddReviewCommandHandler _handler;

        public AddReviewCommandHandlerTests()
        {
            _reviewRepository = Substitute.For<IReviewRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _handler = new AddReviewCommandHandler(_reviewRepository, _unitOfWork, _domainEventDispatcher);
        }

        [Fact]
        public async Task Handle_WhenValidCommand_ShouldCreateReviewSaveAndDispatchEvents()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var rating = 5;
            var content = "This is a very good product and I like it!";
            var command = new AddReviewCommand(customerId, productId, rating, content);

            Review? savedReview = null;
            await _reviewRepository.AddAsync(Arg.Do<Review>(r => savedReview = r), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            savedReview.Should().NotBeNull();
            savedReview!.Id.Value.Should().Be(result.Value);
            savedReview.CustomerId.Value.Should().Be(customerId);
            savedReview.ProductId.Value.Should().Be(productId);
            savedReview.Rating.Should().Be(rating);
            savedReview.Content.Should().Be(content);
            savedReview.Status.Should().Be(ReviewStatus.PendingModeration);

            // Verify Repository Add was called
            await _reviewRepository.Received(1).AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());

            // Verify Unit of Work SaveChangesAsync was called
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

            // Verify Domain Events were dispatched
            await _domainEventDispatcher.Received(1).DispatchAsync(
                Arg.Is<System.Collections.Generic.IEnumerable<IDomainEvent>>(events =>
                    System.Linq.Enumerable.Any(events, e => e is ReviewCreatedEvent && e.AggregateId == result.Value)),
                Arg.Any<CancellationToken>());
        }
    }
}

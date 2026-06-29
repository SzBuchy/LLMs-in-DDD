using System;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Errors;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview
{
    public class AddReviewCommandHandler : CommandHandlerBase<AddReviewCommand, Guid>
    {
        private readonly IReviewRepository _reviewRepository;
        private Review? _review;

        public AddReviewCommandHandler(
            IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _reviewRepository = reviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(AddReviewCommand request, CancellationToken cancellationToken)
        {
            _review = Review.Create(
                Id<Customer>.FromGuid(request.CustomerId),
                Id<Product>.FromGuid(request.ProductId),
                request.Rating,
                request.Content
            );

            await _reviewRepository.AddAsync(_review, cancellationToken);

            return Result.Success<Guid, IDomainError>(_review.Id.Value);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _review;
        }
    }
}

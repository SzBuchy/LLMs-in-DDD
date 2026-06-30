using System;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.BasketContext.Domain.Reviews.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Errors;
using VOEConsulting.Flame.Common.Domain.Exceptions;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditReview
{
    public class EditReviewCommandHandler : CommandHandlerBase<EditReviewCommand, Guid>
    {
        private readonly IReviewRepository _reviewRepository;
        private Review? _review;

        public EditReviewCommandHandler(
            IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _reviewRepository = reviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(EditReviewCommand request, CancellationToken cancellationToken)
        {
            _review = await _reviewRepository.GetByIdAsync(Id<Review>.FromGuid(request.ReviewId), cancellationToken);
            if (_review == null)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Review not found."));
            }

            if (_review.CustomerId.Value != request.CustomerId)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.BadRequest("Customer is not authorized to edit this review."));
            }

            try
            {
                _review.Edit(request.Rating, request.Content);
            }
            catch (Exception ex) when (ex is ValidationException || ex is ArgumentException)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.Validation(ex.Message));
            }

            await _reviewRepository.UpdateAsync(_review, cancellationToken);

            return Result.Success<Guid, IDomainError>(_review.Id.Value);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _review;
        }
    }
}

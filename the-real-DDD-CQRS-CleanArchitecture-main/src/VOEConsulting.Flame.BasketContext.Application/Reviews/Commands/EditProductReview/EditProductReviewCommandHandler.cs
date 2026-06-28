using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditProductReview
{
    public class EditProductReviewCommandHandler : CommandHandlerBase<EditProductReviewCommand, Guid>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private ProductReview? _review;

        public EditProductReviewCommandHandler(
            IProductReviewRepository productReviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(EditProductReviewCommand request, CancellationToken cancellationToken)
        {
            _review = await _productReviewRepository.GetByIdAsync(request.ReviewId);
            if (_review is null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Product review not found."));

            // May throw a ValidationException if the review isn't Published, which the
            // exception-handling pipeline behaviour turns into a Result.Failure.
            _review.Edit(request.Rating, request.Content);

            await _productReviewRepository.UpdateAsync(_review);

            return Result.Success<Guid, IDomainError>(_review.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _review;
        }
    }
}

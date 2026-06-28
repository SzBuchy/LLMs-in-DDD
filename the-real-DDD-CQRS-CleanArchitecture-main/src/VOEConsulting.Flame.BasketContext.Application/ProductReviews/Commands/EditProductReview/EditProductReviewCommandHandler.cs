using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.EditProductReview
{
    public class EditProductReviewCommandHandler : CommandHandlerBase<EditProductReviewCommand, Guid>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private ProductReview? _productReview;

        public EditProductReviewCommandHandler(
            IProductReviewRepository productReviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(
            EditProductReviewCommand request,
            CancellationToken cancellationToken)
        {
            _productReview = await _productReviewRepository.GetByIdAsync(request.ReviewId);
            if (_productReview is null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Product review was not found."));

            if (_productReview.ProductId != request.ProductId)
                return Result.Failure<Guid, IDomainError>(DomainError.BadRequest("Product review does not belong to the given product."));

            _productReview.Edit(
                Id<Customer>.FromGuid(request.CustomerId),
                request.Rating,
                request.Content);

            await _productReviewRepository.UpdateAsync(_productReview);

            return Result.Success<Guid, IDomainError>(_productReview.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _productReview;
        }
    }
}

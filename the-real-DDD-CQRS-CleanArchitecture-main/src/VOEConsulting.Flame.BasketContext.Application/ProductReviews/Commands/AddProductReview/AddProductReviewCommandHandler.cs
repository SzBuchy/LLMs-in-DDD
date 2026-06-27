using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.AddProductReview
{
    public class AddProductReviewCommandHandler : CommandHandlerBase<AddProductReviewCommand, Guid>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private ProductReview? _productReview;

        public AddProductReviewCommandHandler(
            IProductReviewRepository productReviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(
            AddProductReviewCommand request,
            CancellationToken cancellationToken)
        {
            _productReview = ProductReview.Create(
                Id<Customer>.FromGuid(request.CustomerId),
                request.ProductId,
                request.Rating,
                request.Content);

            await _productReviewRepository.AddAsync(_productReview, cancellationToken);

            return Result.Success<Guid, IDomainError>(_productReview.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _productReview;
        }
    }
}

using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Reviews;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.CreateProductReview
{
    public class CreateProductReviewCommandHandler : CommandHandlerBase<CreateProductReviewCommand, Guid>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private ProductReview? _createdReview;

        public CreateProductReviewCommandHandler(
            IProductReviewRepository productReviewRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            var customerId = Id<Customer>.FromGuid(request.CustomerId);

            _createdReview = ProductReview.Create(customerId, request.ProductId, request.Rating, request.Content);

            await _productReviewRepository.AddAsync(_createdReview, cancellationToken);

            return Result.Success<Guid, IDomainError>(_createdReview.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _createdReview;
        }
    }
}

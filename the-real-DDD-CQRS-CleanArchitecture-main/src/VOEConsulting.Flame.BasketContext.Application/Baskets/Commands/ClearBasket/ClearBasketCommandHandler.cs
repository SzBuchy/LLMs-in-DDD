using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.ClearBasket
{
    public class ClearBasketCommandHandler : CommandHandlerBase<ClearBasketCommand, Guid>
    {
        private readonly IBasketRepository _basketRepository;
        private Basket? _basket;

        public ClearBasketCommandHandler(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _basketRepository = basketRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(ClearBasketCommand request, CancellationToken cancellationToken)
        {
            _basket = await _basketRepository.GetByIdAsync(request.BasketId);

            if (_basket == null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Basket not found."));

            _basket.DeleteAll();

            await _basketRepository.UpdateAsync(_basket);

            return Result.Success<Guid, IDomainError>(_basket.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _basket;
        }
    }
}

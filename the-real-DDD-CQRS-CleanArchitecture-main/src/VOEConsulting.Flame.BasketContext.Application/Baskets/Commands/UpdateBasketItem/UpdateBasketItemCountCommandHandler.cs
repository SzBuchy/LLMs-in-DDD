using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.UpdateBasketItem
{
    public class UpdateBasketItemCountCommandHandler : CommandHandlerBase<UpdateBasketItemCountCommand, Guid>
    {
        private readonly IBasketRepository _basketRepository;
        private Basket? _basket;

        public UpdateBasketItemCountCommandHandler(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _basketRepository = basketRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(UpdateBasketItemCountCommand request, CancellationToken cancellationToken)
        {
            _basket = await _basketRepository.GetByIdAsync(request.BasketId);

            if (_basket == null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Basket not found."));

            // Find the item in the basket
            var sellerAndItems = _basket.BasketItems.Values.FirstOrDefault(v => v.Items.Any(i => i.Id.Value == request.ItemId));
            if (sellerAndItems.Items == null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Basket item not found."));

            var basketItem = sellerAndItems.Items.First(i => i.Id.Value == request.ItemId);

            _basket.UpdateItemCount(basketItem, request.Quantity);

            await _basketRepository.UpdateAsync(_basket);

            return Result.Success<Guid, IDomainError>(basketItem.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _basket;
        }
    }
}

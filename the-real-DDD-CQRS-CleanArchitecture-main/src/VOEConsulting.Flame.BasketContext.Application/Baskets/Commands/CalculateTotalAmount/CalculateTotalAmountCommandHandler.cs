using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Repositories;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Baskets.Services;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.CalculateTotalAmount
{
    public class CalculateTotalAmountCommandHandler : CommandHandlerBase<CalculateTotalAmountCommand, decimal>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ICouponService _couponService;
        private Basket? _basket;

        public CalculateTotalAmountCommandHandler(
            IBasketRepository basketRepository,
            ICouponService couponService,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _basketRepository = basketRepository;
            _couponService = couponService;
        }

        protected override async Task<Result<decimal, IDomainError>> ExecuteAsync(CalculateTotalAmountCommand request, CancellationToken cancellationToken)
        {
            _basket = await _basketRepository.GetByIdAsync(request.BasketId);

            if (_basket == null)
                return Result.Failure<decimal, IDomainError>(DomainError.NotFound("Basket not found."));

            await _basket.CalculateTotalAmount(_couponService);

            await _basketRepository.UpdateAsync(_basket);

            return Result.Success<decimal, IDomainError>(_basket.TotalAmount);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<decimal, IDomainError> result)
        {
            return _basket;
        }
    }
}

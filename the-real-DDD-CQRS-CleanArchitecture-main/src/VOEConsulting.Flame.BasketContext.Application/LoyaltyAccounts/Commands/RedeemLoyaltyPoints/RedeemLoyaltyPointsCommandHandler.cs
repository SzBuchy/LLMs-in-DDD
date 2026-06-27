using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Exceptions;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.RedeemLoyaltyPoints
{
    public sealed class RedeemLoyaltyPointsCommandHandler : CommandHandlerBase<RedeemLoyaltyPointsCommand, LoyaltyRedemptionDto>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private LoyaltyAccount? _loyaltyAccount;

        public RedeemLoyaltyPointsCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        protected override async Task<Result<LoyaltyRedemptionDto, IDomainError>> ExecuteAsync(
            RedeemLoyaltyPointsCommand request,
            CancellationToken cancellationToken)
        {
            _loyaltyAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(request.CustomerId);
            if (_loyaltyAccount is null)
                return Result.Failure<LoyaltyRedemptionDto, IDomainError>(DomainError.NotFound("Loyalty account was not found."));

            try
            {
                var redemption = _loyaltyAccount.RedeemPoints(
                    request.OrderId,
                    request.Points,
                    _dateTimeProvider.UtcNow(),
                    request.PointValue);

                await _loyaltyAccountRepository.UpdateAsync(_loyaltyAccount);

                return Result.Success<LoyaltyRedemptionDto, IDomainError>(
                    new LoyaltyRedemptionDto(
                        redemption.Id,
                        redemption.OrderId,
                        redemption.Points,
                        redemption.DiscountAmount,
                        redemption.RedeemedAtUtc));
            }
            catch (ValidationException exception)
            {
                return Result.Failure<LoyaltyRedemptionDto, IDomainError>(DomainError.Validation(exception.Message));
            }
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<LoyaltyRedemptionDto, IDomainError> result) => _loyaltyAccount;
    }
}

using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemLoyaltyPoints
{
    public class RedeemLoyaltyPointsCommandHandler : CommandHandlerBase<RedeemLoyaltyPointsCommand, decimal>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private LoyaltyAccount? _account;

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

        protected override async Task<Result<decimal, IDomainError>> ExecuteAsync(RedeemLoyaltyPointsCommand request, CancellationToken cancellationToken)
        {
            _account = await _loyaltyAccountRepository.GetByIdAsync(request.LoyaltyAccountId);
            if (_account is null)
                return Result.Failure<decimal, IDomainError>(DomainError.NotFound("Loyalty account not found."));

            // May throw a ValidationException (insufficient/expired points) which the
            // exception-handling pipeline behaviour turns into a Result.Failure.
            var discountAmount = _account.RedeemPoints(request.Points, _dateTimeProvider);

            await _loyaltyAccountRepository.UpdateAsync(_account);

            return Result.Success<decimal, IDomainError>(discountAmount);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<decimal, IDomainError> result)
        {
            return _account;
        }
    }
}

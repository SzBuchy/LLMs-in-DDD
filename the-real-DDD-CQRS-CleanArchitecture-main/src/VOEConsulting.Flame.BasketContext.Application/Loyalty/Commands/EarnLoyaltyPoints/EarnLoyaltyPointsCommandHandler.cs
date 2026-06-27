using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.EarnLoyaltyPoints
{
    public class EarnLoyaltyPointsCommandHandler : CommandHandlerBase<EarnLoyaltyPointsCommand, int>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private LoyaltyAccount? _account;

        public EarnLoyaltyPointsCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        protected override async Task<Result<int, IDomainError>> ExecuteAsync(EarnLoyaltyPointsCommand request, CancellationToken cancellationToken)
        {
            _account = await _loyaltyAccountRepository.GetByIdAsync(request.LoyaltyAccountId);
            if (_account is null)
                return Result.Failure<int, IDomainError>(DomainError.NotFound("Loyalty account not found."));

            _account.EarnPoints(request.Points, _dateTimeProvider);

            await _loyaltyAccountRepository.UpdateAsync(_account);

            return Result.Success<int, IDomainError>(_account.GetAvailablePoints(_dateTimeProvider));
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<int, IDomainError> result)
        {
            return _account;
        }
    }
}

using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Services;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.AwardLoyaltyPoints
{
    public sealed class AwardLoyaltyPointsCommandHandler : CommandHandlerBase<AwardLoyaltyPointsCommand, Guid>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private LoyaltyAccount? _loyaltyAccount;

        public AwardLoyaltyPointsCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(
            AwardLoyaltyPointsCommand request,
            CancellationToken cancellationToken)
        {
            _loyaltyAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(request.CustomerId);
            if (_loyaltyAccount is null)
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Loyalty account was not found."));

            _loyaltyAccount.AwardPoints(request.OrderId, request.Points, _dateTimeProvider.UtcNow());
            await _loyaltyAccountRepository.UpdateAsync(_loyaltyAccount);

            return Result.Success<Guid, IDomainError>(_loyaltyAccount.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result) => _loyaltyAccount;
    }
}

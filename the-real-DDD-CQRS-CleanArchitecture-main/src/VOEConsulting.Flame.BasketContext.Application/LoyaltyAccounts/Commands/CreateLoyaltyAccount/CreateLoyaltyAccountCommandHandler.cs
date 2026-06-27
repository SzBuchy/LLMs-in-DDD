using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.CreateLoyaltyAccount
{
    public sealed class CreateLoyaltyAccountCommandHandler : CommandHandlerBase<CreateLoyaltyAccountCommand, Guid>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private LoyaltyAccount? _loyaltyAccount;

        public CreateLoyaltyAccountCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(
            CreateLoyaltyAccountCommand request,
            CancellationToken cancellationToken)
        {
            if (await _loyaltyAccountRepository.IsExistByCustomerIdAsync(request.CustomerId))
                return Result.Failure<Guid, IDomainError>(DomainError.Conflict("Loyalty account already exists for the given customer."));

            _loyaltyAccount = LoyaltyAccount.Create(
                Id<Customer>.FromGuid(request.CustomerId),
                request.MaxPointsPerRedemption);

            await _loyaltyAccountRepository.AddAsync(_loyaltyAccount, cancellationToken);

            return Result.Success<Guid, IDomainError>(_loyaltyAccount.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result) => _loyaltyAccount;
    }
}

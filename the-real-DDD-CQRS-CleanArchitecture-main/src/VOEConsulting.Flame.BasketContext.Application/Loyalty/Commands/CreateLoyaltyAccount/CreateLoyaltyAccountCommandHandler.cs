using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount
{
    public class CreateLoyaltyAccountCommandHandler : CommandHandlerBase<CreateLoyaltyAccountCommand, Guid>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private LoyaltyAccount? _createdAccount;

        public CreateLoyaltyAccountCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(CreateLoyaltyAccountCommand request, CancellationToken cancellationToken)
        {
            if (await _loyaltyAccountRepository.ExistsByCustomerIdAsync(request.CustomerId))
                return Result.Failure<Guid, IDomainError>(DomainError.Conflict("A loyalty account already exists for this customer."));

            _createdAccount = LoyaltyAccount.Create(Id<Customer>.FromGuid(request.CustomerId));

            await _loyaltyAccountRepository.AddAsync(_createdAccount, cancellationToken);

            return Result.Success<Guid, IDomainError>(_createdAccount.Id);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _createdAccount;
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Errors;
using VOEConsulting.Flame.Common.Domain.Events;
using VOEConsulting.Flame.BasketContext.Application.Repositories;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount
{
    public class CreateLoyaltyAccountCommandHandler : CommandHandlerBase<CreateLoyaltyAccountCommand, Guid>
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

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(CreateLoyaltyAccountCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(Id<Customer>.FromGuid(request.CustomerId), cancellationToken);
            if (existingAccount != null)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.Conflict("Loyalty account already exists for the given customer."));
            }

            _loyaltyAccount = LoyaltyAccount.Create(Id<Customer>.FromGuid(request.CustomerId));

            await _loyaltyAccountRepository.AddAsync(_loyaltyAccount, cancellationToken);

            return Result.Success<Guid, IDomainError>(_loyaltyAccount.Id.Value);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _loyaltyAccount;
        }
    }
}

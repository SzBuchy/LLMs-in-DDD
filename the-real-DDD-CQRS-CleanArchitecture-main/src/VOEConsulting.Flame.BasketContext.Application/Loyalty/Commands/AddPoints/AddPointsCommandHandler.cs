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

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.AddPoints
{
    public class AddPointsCommandHandler : CommandHandlerBase<AddPointsCommand, Guid>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private LoyaltyAccount? _loyaltyAccount;

        public AddPointsCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(AddPointsCommand request, CancellationToken cancellationToken)
        {
            _loyaltyAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(Id<Customer>.FromGuid(request.CustomerId), cancellationToken);
            if (_loyaltyAccount == null)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Loyalty account not found for the given customer."));
            }

            _loyaltyAccount.AddPoints(request.Amount, request.EarnedAtUtc ?? DateTimeOffset.UtcNow);

            await _loyaltyAccountRepository.UpdateAsync(_loyaltyAccount, cancellationToken);

            return Result.Success<Guid, IDomainError>(_loyaltyAccount.Id.Value);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _loyaltyAccount;
        }
    }
}

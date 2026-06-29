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

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemPoints
{
    public class RedeemPointsCommandHandler : CommandHandlerBase<RedeemPointsCommand, Guid>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private LoyaltyAccount? _loyaltyAccount;

        public RedeemPointsCommandHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher, unitOfWork)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
        }

        protected override async Task<Result<Guid, IDomainError>> ExecuteAsync(RedeemPointsCommand request, CancellationToken cancellationToken)
        {
            _loyaltyAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(Id<Customer>.FromGuid(request.CustomerId), cancellationToken);
            if (_loyaltyAccount == null)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.NotFound("Loyalty account not found for the given customer."));
            }

            try
            {
                _loyaltyAccount.RedeemPoints(request.Points, request.RedeemedAtUtc ?? DateTimeOffset.UtcNow);
            }
            catch (Exception ex) when (ex is VOEConsulting.Flame.Common.Domain.Exceptions.ValidationException || ex is ArgumentException)
            {
                return Result.Failure<Guid, IDomainError>(DomainError.Validation(ex.Message));
            }

            await _loyaltyAccountRepository.UpdateAsync(_loyaltyAccount, cancellationToken);

            return Result.Success<Guid, IDomainError>(_loyaltyAccount.Id.Value);
        }

        protected override IAggregateRoot? GetAggregateRoot(Result<Guid, IDomainError> result)
        {
            return _loyaltyAccount;
        }
    }
}

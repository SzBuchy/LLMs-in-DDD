using VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount
{
    public class GetLoyaltyAccountQueryHandler : IQueryHandler<GetLoyaltyAccountQuery, LoyaltyAccountDto>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetLoyaltyAccountQueryHandler(ILoyaltyAccountRepository loyaltyAccountRepository, IDateTimeProvider dateTimeProvider)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<LoyaltyAccountDto, IDomainError>> Handle(GetLoyaltyAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _loyaltyAccountRepository.GetByIdAsync(request.LoyaltyAccountId);
            if (account is null)
                return Result.Failure<LoyaltyAccountDto, IDomainError>(DomainError.NotFound("Loyalty account not found."));

            var dto = new LoyaltyAccountDto
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                AvailablePoints = account.GetAvailablePoints(_dateTimeProvider)
            };

            return Result.Success<LoyaltyAccountDto, IDomainError>(dto);
        }
    }
}

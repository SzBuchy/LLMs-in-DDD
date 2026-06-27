using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos;
using VOEConsulting.Flame.Common.Domain.Services;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount
{
    public sealed class GetLoyaltyAccountQueryHandler : IQueryHandler<GetLoyaltyAccountQuery, LoyaltyAccountDto>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GetLoyaltyAccountQueryHandler(
            ILoyaltyAccountRepository loyaltyAccountRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<LoyaltyAccountDto, IDomainError>> Handle(
            GetLoyaltyAccountQuery request,
            CancellationToken cancellationToken)
        {
            var loyaltyAccount = await _loyaltyAccountRepository.GetByCustomerIdAsync(request.CustomerId);
            if (loyaltyAccount is null)
                return Result.Failure<LoyaltyAccountDto, IDomainError>(DomainError.NotFound("Loyalty account was not found."));

            var now = _dateTimeProvider.UtcNow();

            var pointBatches = loyaltyAccount.PointBatches
                .OrderBy(batch => batch.ExpiresAtUtc)
                .Select(batch => new LoyaltyPointBatchDto(
                    batch.Id,
                    batch.OrderId,
                    batch.Points,
                    batch.RedeemedPoints,
                    batch.AvailablePoints(now),
                    batch.AwardedAtUtc,
                    batch.ExpiresAtUtc,
                    batch.ExpiredAtUtc))
                .ToList();

            var redemptions = loyaltyAccount.Redemptions
                .OrderByDescending(redemption => redemption.RedeemedAtUtc)
                .Select(redemption => new LoyaltyRedemptionDto(
                    redemption.Id,
                    redemption.OrderId,
                    redemption.Points,
                    redemption.DiscountAmount,
                    redemption.RedeemedAtUtc))
                .ToList();

            return Result.Success<LoyaltyAccountDto, IDomainError>(
                new LoyaltyAccountDto(
                    loyaltyAccount.Id,
                    loyaltyAccount.CustomerId,
                    loyaltyAccount.MaxPointsPerRedemption,
                    loyaltyAccount.AvailablePoints(now),
                    loyaltyAccount.ExpiredPoints(now),
                    pointBatches,
                    redemptions));
        }
    }
}

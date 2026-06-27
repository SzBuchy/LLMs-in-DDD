using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount
{
    public sealed record GetLoyaltyAccountQuery(Guid CustomerId) : IQuery<LoyaltyAccountDto>;
}

using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Dtos;
using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.RedeemLoyaltyPoints
{
    public sealed record RedeemLoyaltyPointsCommand(
        Guid CustomerId,
        Guid OrderId,
        int Points,
        decimal PointValue = LoyaltyAccount.DefaultPointValue) : ICommand<LoyaltyRedemptionDto>;
}

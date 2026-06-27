using VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.CreateLoyaltyAccount
{
    public sealed record CreateLoyaltyAccountCommand(
        Guid CustomerId,
        int MaxPointsPerRedemption = LoyaltyAccount.DefaultMaxPointsPerRedemption) : ICommand<Guid>;
}

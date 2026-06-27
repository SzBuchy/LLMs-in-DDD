namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.EarnLoyaltyPoints
{
    public record EarnLoyaltyPointsCommand(Guid LoyaltyAccountId, int Points) : ICommand<int>;
}

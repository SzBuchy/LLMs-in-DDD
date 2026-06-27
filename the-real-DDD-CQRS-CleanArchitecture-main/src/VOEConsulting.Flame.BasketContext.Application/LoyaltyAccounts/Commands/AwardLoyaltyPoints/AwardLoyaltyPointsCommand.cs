namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.AwardLoyaltyPoints
{
    public sealed record AwardLoyaltyPointsCommand(
        Guid CustomerId,
        Guid OrderId,
        int Points) : ICommand<Guid>;
}

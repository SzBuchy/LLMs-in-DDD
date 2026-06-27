namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemLoyaltyPoints
{
    public record RedeemLoyaltyPointsCommand(Guid LoyaltyAccountId, int Points) : ICommand<decimal>;
}

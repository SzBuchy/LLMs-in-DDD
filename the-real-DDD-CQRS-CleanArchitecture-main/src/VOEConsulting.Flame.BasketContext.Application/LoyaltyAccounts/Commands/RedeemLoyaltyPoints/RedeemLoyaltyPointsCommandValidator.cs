using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.RedeemLoyaltyPoints
{
    public sealed class RedeemLoyaltyPointsCommandValidator : AbstractValidator<RedeemLoyaltyPointsCommand>
    {
        public RedeemLoyaltyPointsCommandValidator()
        {
            RuleFor(command => command.CustomerId).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.Points).GreaterThan(0);
            RuleFor(command => command.PointValue).GreaterThan(0);
        }
    }
}

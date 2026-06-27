using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.AwardLoyaltyPoints
{
    public sealed class AwardLoyaltyPointsCommandValidator : AbstractValidator<AwardLoyaltyPointsCommand>
    {
        public AwardLoyaltyPointsCommandValidator()
        {
            RuleFor(command => command.CustomerId).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
            RuleFor(command => command.Points).GreaterThan(0);
        }
    }
}

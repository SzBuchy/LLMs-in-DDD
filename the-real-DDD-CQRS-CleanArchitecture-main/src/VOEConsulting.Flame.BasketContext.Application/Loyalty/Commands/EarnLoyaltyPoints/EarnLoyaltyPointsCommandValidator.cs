using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.EarnLoyaltyPoints
{
    public class EarnLoyaltyPointsCommandValidator : AbstractValidator<EarnLoyaltyPointsCommand>
    {
        public EarnLoyaltyPointsCommandValidator()
        {
            RuleFor(x => x.LoyaltyAccountId).NotEmpty();
            RuleFor(x => x.Points).GreaterThan(0);
        }
    }
}

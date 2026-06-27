using FluentValidation;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemLoyaltyPoints
{
    public class RedeemLoyaltyPointsCommandValidator : AbstractValidator<RedeemLoyaltyPointsCommand>
    {
        public RedeemLoyaltyPointsCommandValidator()
        {
            RuleFor(x => x.LoyaltyAccountId).NotEmpty();
            RuleFor(x => x.Points).InclusiveBetween(1, LoyaltyAccount.MaxPointsRedeemablePerTransaction);
        }
    }
}

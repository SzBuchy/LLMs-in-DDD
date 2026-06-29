using FluentValidation;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemPoints
{
    public class RedeemPointsCommandValidator : AbstractValidator<RedeemPointsCommand>
    {
        public RedeemPointsCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Points)
                .GreaterThan(0)
                .LessThanOrEqualTo(LoyaltyAccount.MaxPointsPerRedemption)
                .WithMessage($"Cannot redeem more than {LoyaltyAccount.MaxPointsPerRedemption} points at once.");
        }
    }
}

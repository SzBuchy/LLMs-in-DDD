using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.CreateLoyaltyAccount
{
    public sealed class CreateLoyaltyAccountCommandValidator : AbstractValidator<CreateLoyaltyAccountCommand>
    {
        public CreateLoyaltyAccountCommandValidator()
        {
            RuleFor(command => command.CustomerId).NotEmpty();
            RuleFor(command => command.MaxPointsPerRedemption).GreaterThan(0);
        }
    }
}

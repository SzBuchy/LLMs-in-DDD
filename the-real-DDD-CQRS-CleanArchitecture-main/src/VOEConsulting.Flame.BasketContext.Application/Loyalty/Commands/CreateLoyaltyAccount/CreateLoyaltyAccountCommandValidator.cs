using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount
{
    public class CreateLoyaltyAccountCommandValidator : AbstractValidator<CreateLoyaltyAccountCommand>
    {
        public CreateLoyaltyAccountCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }
}

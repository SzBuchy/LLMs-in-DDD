using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.AddPoints
{
    public class AddPointsCommandValidator : AbstractValidator<AddPointsCommand>
    {
        public AddPointsCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}

using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview
{
    public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
    {
        public AddReviewCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Content).NotEmpty().Length(10, 500);
        }
    }
}

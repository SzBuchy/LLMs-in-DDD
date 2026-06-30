using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditReview
{
    public class EditReviewCommandValidator : AbstractValidator<EditReviewCommand>
    {
        public EditReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Content).NotEmpty().Length(10, 500);
        }
    }
}

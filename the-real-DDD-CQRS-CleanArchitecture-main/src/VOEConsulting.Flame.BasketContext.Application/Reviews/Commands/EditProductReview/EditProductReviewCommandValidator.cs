using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditProductReview
{
    public class EditProductReviewCommandValidator : AbstractValidator<EditProductReviewCommand>
    {
        public EditProductReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Content).NotEmpty().Length(10, 500);
        }
    }
}

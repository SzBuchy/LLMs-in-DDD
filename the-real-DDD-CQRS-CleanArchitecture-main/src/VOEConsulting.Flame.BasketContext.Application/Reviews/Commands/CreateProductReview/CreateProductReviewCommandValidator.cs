using FluentValidation;

namespace VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.CreateProductReview
{
    public class CreateProductReviewCommandValidator : AbstractValidator<CreateProductReviewCommand>
    {
        public CreateProductReviewCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5);
            RuleFor(x => x.Content).NotEmpty().Length(10, 500);
        }
    }
}

using FluentValidation;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;

namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.AddProductReview
{
    public class AddProductReviewCommandValidator : AbstractValidator<AddProductReviewCommand>
    {
        public AddProductReviewCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Rating)
                .InclusiveBetween(ProductReview.MinRating, ProductReview.MaxRating);
            RuleFor(x => x.Content)
                .NotEmpty()
                .MinimumLength(ProductReview.MinContentLength)
                .MaximumLength(ProductReview.MaxContentLength);
        }
    }
}

using FluentValidation;
using VOEConsulting.Flame.BasketContext.Domain.ProductReviews;

namespace VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.EditProductReview
{
    public class EditProductReviewCommandValidator : AbstractValidator<EditProductReviewCommand>
    {
        public EditProductReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
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

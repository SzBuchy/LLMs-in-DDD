using FastEndpoints;
using FluentValidation;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class UpdateReviewValidator : Validator<UpdateReviewRequest>
{
    public UpdateReviewValidator()
    {
        RuleFor(request => request.CatalogItemId)
            .GreaterThan(0);

        RuleFor(request => request.ReviewId)
            .GreaterThan(0);

        RuleFor(request => request.Rating)
            .InclusiveBetween(Review.MinRating, Review.MaxRating);

        RuleFor(request => request.Content)
            .NotEmpty()
            .MinimumLength(Review.MinContentLength)
            .MaximumLength(Review.MaxContentLength);
    }
}

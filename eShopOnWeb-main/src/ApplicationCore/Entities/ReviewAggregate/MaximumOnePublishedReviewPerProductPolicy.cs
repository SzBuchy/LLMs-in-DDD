using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public class MaximumOnePublishedReviewPerProductPolicy : IReviewPublicationPolicy
{
    public static MaximumOnePublishedReviewPerProductPolicy Instance { get; } = new();

    public void EnsureCanPublish(Review review, IEnumerable<Review> existingReviews)
    {
        Guard.Against.Null(review, nameof(review));
        Guard.Against.Null(existingReviews, nameof(existingReviews));

        var alreadyHasPublishedReview = existingReviews.Any(existingReview =>
            !IsSameReview(review, existingReview) &&
            existingReview.BuyerId == review.BuyerId &&
            existingReview.CatalogItemId == review.CatalogItemId &&
            existingReview.Status == ReviewStatus.Published);

        if (alreadyHasPublishedReview)
        {
            throw new InvalidOperationException(
                "Customer can have only one published review for a product. Withdraw the previous review before publishing another one.");
        }
    }

    private static bool IsSameReview(Review review, Review existingReview)
    {
        if (ReferenceEquals(review, existingReview))
        {
            return true;
        }

        return review.Id != 0 && review.Id == existingReview.Id;
    }
}

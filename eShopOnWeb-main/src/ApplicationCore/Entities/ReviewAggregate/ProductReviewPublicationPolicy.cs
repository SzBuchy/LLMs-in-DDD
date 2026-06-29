using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public class ProductReviewPublicationPolicy : IProductReviewPublicationPolicy
{
    public void Validate(ProductReview reviewToPublish, IEnumerable<ProductReview> existingReviews)
    {
        var hasPublishedReview = existingReviews.Any(r =>
            r.CustomerId == reviewToPublish.CustomerId &&
            r.CatalogItemId == reviewToPublish.CatalogItemId &&
            r.Status == ReviewStatus.Published &&
            !IsSameReview(r, reviewToPublish));

        if (hasPublishedReview)
        {
            throw new InvalidOperationException("Customer already has a published review for this product. The existing review must be withdrawn first.");
        }
    }

    private bool IsSameReview(ProductReview a, ProductReview b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }
        if (a.Id != 0 && b.Id != 0 && a.Id == b.Id)
        {
            return true;
        }
        return false;
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

// Domain policy: a buyer may have at most one Published review per product at a time.
// Publishing a new review while a previous one from the same buyer for the same product
// is still Published is not allowed - the previous review must be withdrawn first.
public class SingleActivePublishedReviewPolicy : IReviewPublicationPolicy
{
    public void EnsureCanPublish(Review review, IEnumerable<Review> buyerReviewsForSameProduct)
    {
        var hasOtherPublishedReview = buyerReviewsForSameProduct
            .Any(other => other != review
                && other.BuyerId == review.BuyerId
                && other.CatalogItemId == review.CatalogItemId
                && other.Status == ReviewStatus.Published);

        if (hasOtherPublishedReview)
        {
            throw new ReviewAlreadyPublishedException(review.BuyerId, review.CatalogItemId);
        }
    }
}

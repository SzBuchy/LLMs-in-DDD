using System.Collections.Generic;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public interface IReviewPublicationPolicy
{
    void EnsureCanPublish(Review review, IEnumerable<Review> existingReviews);
}

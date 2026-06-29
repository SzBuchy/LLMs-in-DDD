using System.Collections.Generic;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

public interface IProductReviewPublicationPolicy
{
    void Validate(ProductReview reviewToPublish, IEnumerable<ProductReview> existingReviews);
}

using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ProductReviewNotFoundException : Exception
{
    public ProductReviewNotFoundException(int reviewId) : base($"No product review found with id {reviewId}")
    {
    }
}

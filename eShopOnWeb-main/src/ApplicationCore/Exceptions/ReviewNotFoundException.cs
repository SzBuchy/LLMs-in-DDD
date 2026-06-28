using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ReviewNotFoundException : Exception
{
    public ReviewNotFoundException(int reviewId)
        : base($"No review found with id {reviewId}")
    {
    }
}

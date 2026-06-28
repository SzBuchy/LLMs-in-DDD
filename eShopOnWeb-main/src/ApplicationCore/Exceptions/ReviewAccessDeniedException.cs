using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ReviewAccessDeniedException : Exception
{
    public ReviewAccessDeniedException(int reviewId)
        : base($"Current customer cannot edit review with id {reviewId}")
    {
    }
}

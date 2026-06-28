using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ReviewNotEditableException : InvalidOperationException
{
    public ReviewNotEditableException(ReviewStatus status)
        : base($"Cannot edit a review with status '{status}'. Only '{ReviewStatus.Published}' reviews can be edited.")
    {
    }
}

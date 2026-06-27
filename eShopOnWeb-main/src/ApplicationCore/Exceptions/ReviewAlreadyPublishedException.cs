using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class ReviewAlreadyPublishedException : Exception
{
    public ReviewAlreadyPublishedException(string buyerId, int catalogItemId)
        : base($"Buyer '{buyerId}' already has a published review for catalog item {catalogItemId}. " +
               "Withdraw the existing review before publishing a new one.")
    {
    }
}

using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class LoyaltyAccountNotFoundException : Exception
{
    public LoyaltyAccountNotFoundException(string buyerId)
        : base($"No loyalty account found for buyer '{buyerId}'.")
    {
    }
}

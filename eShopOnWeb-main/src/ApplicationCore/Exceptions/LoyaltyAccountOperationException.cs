using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class LoyaltyAccountOperationException : Exception
{
    public LoyaltyAccountOperationException(string message) : base(message)
    {
    }
}

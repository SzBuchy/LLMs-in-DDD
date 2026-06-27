using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class InsufficientLoyaltyPointsException : Exception
{
    public InsufficientLoyaltyPointsException(string buyerId, int requestedPoints, int availablePoints)
        : base($"Buyer '{buyerId}' requested to redeem {requestedPoints} points but only has {availablePoints} available.")
    {
    }
}

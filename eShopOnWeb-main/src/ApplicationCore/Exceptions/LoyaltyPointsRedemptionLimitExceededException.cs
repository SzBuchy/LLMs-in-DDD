using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions;

public class LoyaltyPointsRedemptionLimitExceededException : Exception
{
    public LoyaltyPointsRedemptionLimitExceededException(int requestedPoints, int maxAllowed)
        : base($"Cannot redeem {requestedPoints} points in a single transaction; the maximum allowed is {maxAllowed}.")
    {
    }
}

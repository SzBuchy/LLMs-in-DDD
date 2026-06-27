using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class RedeemLoyaltyPointsResponse : BaseResponse
{
    public RedeemLoyaltyPointsResponse(Guid correlationId) : base(correlationId)
    {
    }

    public RedeemLoyaltyPointsResponse()
    {
    }

    public int RedeemedPoints { get; set; }
    public decimal DiscountAmount { get; set; }
}

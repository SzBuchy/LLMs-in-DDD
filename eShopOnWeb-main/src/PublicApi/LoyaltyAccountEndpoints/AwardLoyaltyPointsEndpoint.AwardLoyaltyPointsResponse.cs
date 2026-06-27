using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardLoyaltyPointsResponse : BaseResponse
{
    public AwardLoyaltyPointsResponse(Guid correlationId) : base(correlationId)
    {
    }

    public AwardLoyaltyPointsResponse()
    {
    }

    public int AwardedPoints { get; set; }
    public LoyaltyAccountDto LoyaltyAccount { get; set; } = new();
}

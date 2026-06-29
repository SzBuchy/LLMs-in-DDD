using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class SpendPointsResponse : BaseResponse
{
    public SpendPointsResponse(Guid correlationId) : base(correlationId) {}
    public SpendPointsResponse() {}

    public LoyaltyAccountDto LoyaltyAccount { get; set; } = new LoyaltyAccountDto();
}

using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardPointsResponse : BaseResponse
{
    public AwardPointsResponse(Guid correlationId) : base(correlationId) {}
    public AwardPointsResponse() {}

    public LoyaltyAccountDto LoyaltyAccount { get; set; } = new LoyaltyAccountDto();
}

using System;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class GetLoyaltyAccountResponse : BaseResponse
{
    public GetLoyaltyAccountResponse(Guid correlationId) : base(correlationId) {}
    public GetLoyaltyAccountResponse() {}

    public LoyaltyAccountDto LoyaltyAccount { get; set; } = new LoyaltyAccountDto();
}

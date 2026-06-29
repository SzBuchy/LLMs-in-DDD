namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardPointsRequest : BaseRequest
{
    public int Points { get; set; }
    public string? OrderId { get; set; }
}

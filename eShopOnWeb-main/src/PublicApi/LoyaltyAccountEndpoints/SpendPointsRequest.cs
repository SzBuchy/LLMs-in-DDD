namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class SpendPointsRequest : BaseRequest
{
    public int Points { get; set; }
    public string? OrderId { get; set; }
}

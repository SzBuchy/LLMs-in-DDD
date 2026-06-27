namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardLoyaltyPointsRequest : BaseRequest
{
    public int OrderId { get; set; }
    public decimal OrderTotal { get; set; }
}

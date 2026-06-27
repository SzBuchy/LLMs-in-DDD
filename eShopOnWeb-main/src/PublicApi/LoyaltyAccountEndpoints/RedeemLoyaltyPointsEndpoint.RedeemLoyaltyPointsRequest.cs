namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class RedeemLoyaltyPointsRequest : BaseRequest
{
    public int PointsToRedeem { get; set; }
}

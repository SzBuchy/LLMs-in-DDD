using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class RedeemLoyaltyPointsRequest : BaseRequest
{
    [Range(1, int.MaxValue)]
    public int Points { get; set; }
}

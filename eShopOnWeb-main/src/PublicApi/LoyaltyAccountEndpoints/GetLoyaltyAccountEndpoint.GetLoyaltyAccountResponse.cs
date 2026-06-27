namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class GetLoyaltyAccountResponse : BaseResponse
{
    public string BuyerId { get; set; }
    public int AvailablePoints { get; set; }
}

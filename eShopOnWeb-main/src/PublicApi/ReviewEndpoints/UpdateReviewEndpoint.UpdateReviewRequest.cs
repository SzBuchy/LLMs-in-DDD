namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class UpdateReviewRequest : BaseRequest
{
    public int CatalogItemId { get; set; }
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
}

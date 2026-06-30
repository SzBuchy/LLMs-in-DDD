namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class EditProductReviewRequest : BaseRequest
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public string TextContent { get; set; } = string.Empty;
}

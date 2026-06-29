namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class CreateProductReviewRequest : BaseRequest
{
    public int CatalogItemId { get; set; }
    public int Rating { get; set; }
    public string TextContent { get; set; }
}

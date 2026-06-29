namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class ProductReviewDto
{
    public int Id { get; set; }
    public string CustomerId { get; set; }
    public int CatalogItemId { get; set; }
    public int Rating { get; set; }
    public string TextContent { get; set; }
    public string Status { get; set; }
}

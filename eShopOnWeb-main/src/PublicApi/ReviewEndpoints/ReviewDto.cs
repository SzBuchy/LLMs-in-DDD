namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class ReviewDto
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public int CatalogItemId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public string Status { get; set; }
}

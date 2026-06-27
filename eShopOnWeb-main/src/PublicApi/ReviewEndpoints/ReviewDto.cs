using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class ReviewDto
{
    public int Id { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public int CatalogItemId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = string.Empty;
    public ReviewStatus Status { get; set; }
}

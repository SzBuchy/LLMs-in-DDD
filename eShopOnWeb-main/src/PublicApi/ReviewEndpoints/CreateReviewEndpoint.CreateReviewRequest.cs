using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class CreateReviewRequest : BaseRequest
{
    [Range(1, int.MaxValue)]
    public int CatalogItemId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required]
    public string Content { get; set; }
}

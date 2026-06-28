using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class EditReviewRequest : BaseRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    [Required]
    public string Content { get; set; }
}

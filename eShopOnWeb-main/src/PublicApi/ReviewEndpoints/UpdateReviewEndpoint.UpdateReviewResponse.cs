using System;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class UpdateReviewResponse : BaseResponse
{
    public UpdateReviewResponse(Guid correlationId) : base(correlationId)
    {
    }

    public UpdateReviewResponse()
    {
    }

    public ReviewDto Review { get; set; } = new();
}

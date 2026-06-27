using System;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class CreateReviewResponse : BaseResponse
{
    public CreateReviewResponse(Guid correlationId) : base(correlationId)
    {
    }

    public CreateReviewResponse()
    {
    }

    public ReviewDto Review { get; set; }
}

using System;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

public class EditReviewResponse : BaseResponse
{
    public EditReviewResponse(Guid correlationId) : base(correlationId)
    {
    }

    public EditReviewResponse()
    {
    }

    public ReviewDto Review { get; set; }
}

using System;

namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class EditProductReviewResponse : BaseResponse
{
    public EditProductReviewResponse(Guid correlationId) : base(correlationId) {}
    public EditProductReviewResponse() {}

    public ProductReviewDto ProductReview { get; set; } = new();
}

using System;

namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class CreateProductReviewResponse : BaseResponse
{
    public CreateProductReviewResponse(Guid correlationId) : base(correlationId) {}
    public CreateProductReviewResponse() {}

    public ProductReviewDto ProductReview { get; set; } = new();
}

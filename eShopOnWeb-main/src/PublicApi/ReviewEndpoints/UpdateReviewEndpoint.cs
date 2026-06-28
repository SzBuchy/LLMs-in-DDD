using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

/// <summary>
/// Updates a published product review for the authenticated customer and sends it back to moderation.
/// </summary>
public class UpdateReviewEndpoint(IReviewService reviewService)
    : Endpoint<UpdateReviewRequest, UpdateReviewResponse>
{
    public override void Configure()
    {
        Put("api/catalog-items/{catalogItemId}/reviews/{reviewId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<UpdateReviewResponse>()
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("ReviewEndpoints"));
    }

    public override async Task HandleAsync(UpdateReviewRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var review = await reviewService.EditReviewAsync(
            buyerId,
            request.CatalogItemId,
            request.ReviewId,
            request.Rating,
            request.Content,
            ct);

        var response = new UpdateReviewResponse(request.CorrelationId())
        {
            Review = new ReviewDto
            {
                Id = review.Id,
                BuyerId = review.BuyerId,
                CatalogItemId = review.CatalogItemId,
                Rating = review.Rating,
                Content = review.Content,
                Status = review.Status
            }
        };

        await SendAsync(response, cancellation: ct);
    }
}

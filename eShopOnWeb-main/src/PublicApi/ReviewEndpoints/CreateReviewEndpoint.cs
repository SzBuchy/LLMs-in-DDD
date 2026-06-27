using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

/// <summary>
/// Creates a new product review for the authenticated customer.
/// </summary>
public class CreateReviewEndpoint(IReviewService reviewService)
    : Endpoint<CreateReviewRequest, CreateReviewResponse>
{
    public override void Configure()
    {
        Post("api/catalog-items/{catalogItemId}/reviews");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<CreateReviewResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .WithTags("ReviewEndpoints"));
    }

    public override async Task HandleAsync(CreateReviewRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var review = await reviewService.AddReviewAsync(
            buyerId,
            request.CatalogItemId,
            request.Rating,
            request.Content,
            ct);

        var response = new CreateReviewResponse(request.CorrelationId())
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

        await SendAsync(response, StatusCodes.Status201Created, ct);
    }
}

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.ReviewEndpoints;

/// <summary>
/// Edits an already published Product Review submitted by the currently authenticated
/// customer. The edited review is sent back for moderation.
/// </summary>
public class EditReviewEndpoint(IReviewService reviewService)
    : Endpoint<EditReviewRequest, Results<Ok<EditReviewResponse>, NotFound>>
{
    public override void Configure()
    {
        Put("api/reviews");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<EditReviewResponse>()
             .Produces(StatusCodes.Status404NotFound)
             .WithTags("ReviewEndpoints"));
    }

    public override async Task<Results<Ok<EditReviewResponse>, NotFound>> ExecuteAsync(EditReviewRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name);

        Review review;
        try
        {
            review = await reviewService.EditReviewAsync(buyerId, request.Id, request.Rating, request.Content);
        }
        catch (ReviewNotFoundException)
        {
            return TypedResults.NotFound();
        }

        var response = new EditReviewResponse(request.CorrelationId())
        {
            Review = new ReviewDto
            {
                Id = review.Id,
                BuyerId = review.BuyerId,
                CatalogItemId = review.CatalogItemId,
                Rating = review.Rating,
                Content = review.Content,
                Status = review.Status.ToString()
            }
        };

        return TypedResults.Ok(response);
    }
}

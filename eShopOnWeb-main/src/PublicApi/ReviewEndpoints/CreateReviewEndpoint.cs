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
/// Adds a new Product Review submitted by the currently authenticated customer
/// </summary>
public class CreateReviewEndpoint(IReviewService reviewService)
    : Endpoint<CreateReviewRequest, Results<Created<CreateReviewResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("api/reviews");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<CreateReviewResponse>(StatusCodes.Status201Created)
             .Produces(StatusCodes.Status404NotFound)
             .WithTags("ReviewEndpoints"));
    }

    public override async Task<Results<Created<CreateReviewResponse>, NotFound>> ExecuteAsync(CreateReviewRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name);

        Review review;
        try
        {
            review = await reviewService.AddReviewAsync(buyerId, request.CatalogItemId, request.Rating, request.Content);
        }
        catch (CatalogItemNotFoundException)
        {
            return TypedResults.NotFound();
        }

        var response = new CreateReviewResponse(request.CorrelationId())
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

        return TypedResults.Created($"api/reviews/{review.Id}", response);
    }
}

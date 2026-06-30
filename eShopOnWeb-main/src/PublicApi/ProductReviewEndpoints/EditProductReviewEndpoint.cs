using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class EditProductReviewEndpoint(IProductReviewService productReviewService)
    : Endpoint<EditProductReviewRequest, EditProductReviewResponse>
{
    public override void Configure()
    {
        Put("api/product-reviews");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
        {
            d.Produces<EditProductReviewResponse>(StatusCodes.Status200OK);
            d.Produces(StatusCodes.Status400BadRequest);
            d.Produces(StatusCodes.Status401Unauthorized);
            d.Produces(StatusCodes.Status404NotFound);
            d.WithTags("ProductReviewEndpoints");
        });
    }

    public override async Task HandleAsync(EditProductReviewRequest request, CancellationToken ct)
    {
        var response = new EditProductReviewResponse(request.CorrelationId());

        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        try
        {
            var review = await productReviewService.EditProductReviewAsync(
                customerId,
                request.ReviewId,
                request.Rating,
                request.TextContent,
                ct);

            response.ProductReview = new ProductReviewDto
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                CatalogItemId = review.CatalogItemId,
                Rating = review.Rating,
                TextContent = review.TextContent,
                Status = review.Status.ToString()
            };

            await SendAsync(response, statusCode: 200, cancellation: ct);
        }
        catch (ProductReviewNotFoundException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 404, cancellation: ct);
        }
        catch (ArgumentException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 400, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 400, cancellation: ct);
        }
    }
}

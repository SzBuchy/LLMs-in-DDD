using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;

public class CreateProductReviewEndpoint(IProductReviewService productReviewService)
    : Endpoint<CreateProductReviewRequest, CreateProductReviewResponse>
{
    public override void Configure()
    {
        Post("api/product-reviews");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
        {
            d.Produces<CreateProductReviewResponse>(StatusCodes.Status201Created);
            d.Produces(StatusCodes.Status400BadRequest);
            d.Produces(StatusCodes.Status401Unauthorized);
            d.WithTags("ProductReviewEndpoints");
        });
    }

    public override async Task HandleAsync(CreateProductReviewRequest request, CancellationToken ct)
    {
        var response = new CreateProductReviewResponse(request.CorrelationId());

        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        try
        {
            var review = await productReviewService.AddProductReviewAsync(
                customerId,
                request.CatalogItemId,
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

            await SendAsync(response, statusCode: 201, cancellation: ct);
        }
        catch (CatalogItemNotFoundException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 400, cancellation: ct);
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

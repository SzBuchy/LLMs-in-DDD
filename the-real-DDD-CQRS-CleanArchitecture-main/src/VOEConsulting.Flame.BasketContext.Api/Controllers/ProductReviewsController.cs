using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.AddProductReview;
using VOEConsulting.Flame.BasketContext.Application.ProductReviews.Commands.EditProductReview;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/products/{productId:guid}/reviews")]
    public class ProductReviewsController(ISender sender, ILogger<ProductReviewsController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> AddProductReview(
            Guid productId,
            [FromBody] AddProductReviewCommand command)
        {
            if (productId != command.ProductId)
                return BadRequest("Product ID in URL does not match the command.");

            var result = await _sender.Send(command);

            return result.IsSuccess
                ? Created($"/api/product-reviews/{result.Value}", result.Value)
                : HandleError(result.Error);
        }

        [HttpPut("{reviewId:guid}")]
        public async Task<IActionResult> EditProductReview(
            Guid productId,
            Guid reviewId,
            [FromBody] EditProductReviewCommand command)
        {
            if (productId != command.ProductId)
                return BadRequest("Product ID in URL does not match the command.");

            if (reviewId != command.ReviewId)
                return BadRequest("Review ID in URL does not match the command.");

            var result = await _sender.Send(command);

            return result.IsSuccess
                ? NoContent()
                : HandleError(result.Error);
        }
    }
}

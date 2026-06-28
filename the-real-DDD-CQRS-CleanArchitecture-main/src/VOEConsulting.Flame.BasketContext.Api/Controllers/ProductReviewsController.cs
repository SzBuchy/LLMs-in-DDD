using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.CreateProductReview;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditProductReview;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewsController(ISender sender, ILogger<ProductReviewsController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        // POST api/productreviews
        [HttpPost]
        public async Task<IActionResult> CreateProductReview([FromBody] CreateProductReviewCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Created($"api/productreviews/{result.Value}", result.Value);

            return HandleError(result.Error);
        }

        // PUT api/productreviews/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProductReview(Guid id, [FromBody] EditProductReviewCommand command)
        {
            if (id != command.ReviewId)
                return BadRequest("Review ID in URL does not match the command.");

            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }
    }
}

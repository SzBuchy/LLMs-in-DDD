using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.CreateProductReview;

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
    }
}

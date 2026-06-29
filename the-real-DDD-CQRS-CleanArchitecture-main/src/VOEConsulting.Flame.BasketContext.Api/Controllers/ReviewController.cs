using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController(ISender sender, ILogger<ReviewController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddReviewCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }
    }
}

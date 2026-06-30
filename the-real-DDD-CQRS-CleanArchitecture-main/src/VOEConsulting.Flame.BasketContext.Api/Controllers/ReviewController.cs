using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.AddReview;
using VOEConsulting.Flame.BasketContext.Application.Reviews.Commands.EditReview;

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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditReview(Guid id, [FromBody] EditReviewCommand command)
        {
            if (id != command.ReviewId)
                return BadRequest("Review ID in URL does not match the command.");

            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }
    }
}

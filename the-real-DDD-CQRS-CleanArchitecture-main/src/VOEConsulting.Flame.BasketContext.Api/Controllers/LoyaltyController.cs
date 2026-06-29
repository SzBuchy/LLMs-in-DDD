using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.AddPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyController(ISender sender, ILogger<LoyaltyController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateLoyaltyAccountCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpPost("points/add")]
        public async Task<IActionResult> AddPoints([FromBody] AddPointsCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpPost("points/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAccount(Guid customerId)
        {
            var query = new GetLoyaltyAccountQuery(customerId);
            var result = await _sender.Send(query);
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.EarnLoyaltyPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemLoyaltyPoints;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyAccountsController(ISender sender, ILogger<LoyaltyAccountsController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        // POST api/loyaltyaccounts
        [HttpPost]
        public async Task<IActionResult> CreateLoyaltyAccount([FromBody] CreateLoyaltyAccountCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetLoyaltyAccount), new { id = result.Value }, result.Value);

            return HandleError(result.Error);
        }

        // GET api/loyaltyaccounts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoyaltyAccount(Guid id)
        {
            var result = await _sender.Send(new GetLoyaltyAccountQuery(id));
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }

        // POST api/loyaltyaccounts/{id}/earn
        [HttpPost("{id}/earn")]
        public async Task<IActionResult> EarnPoints(Guid id, [FromBody] EarnLoyaltyPointsCommand command)
        {
            if (id != command.LoyaltyAccountId)
                return BadRequest("Loyalty account ID in URL does not match the command.");

            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }

        // POST api/loyaltyaccounts/{id}/redeem
        [HttpPost("{id}/redeem")]
        public async Task<IActionResult> RedeemPoints(Guid id, [FromBody] RedeemLoyaltyPointsCommand command)
        {
            if (id != command.LoyaltyAccountId)
                return BadRequest("Loyalty account ID in URL does not match the command.");

            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }
    }
}

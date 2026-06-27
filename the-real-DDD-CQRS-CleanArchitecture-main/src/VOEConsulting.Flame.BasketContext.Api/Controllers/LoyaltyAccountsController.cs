using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.AwardLoyaltyPoints;
using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.CreateLoyaltyAccount;
using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Commands.RedeemLoyaltyPoints;
using VOEConsulting.Flame.BasketContext.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/customers/{customerId:guid}/loyalty-account")]
    public class LoyaltyAccountsController(ISender sender, ILogger<LoyaltyAccountsController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetLoyaltyAccount(Guid customerId)
        {
            var result = await _sender.Send(new GetLoyaltyAccountQuery(customerId));
            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoyaltyAccount(
            Guid customerId,
            [FromBody] CreateLoyaltyAccountCommand command)
        {
            if (customerId != command.CustomerId)
                return BadRequest("Customer ID in URL does not match the command.");

            var result = await _sender.Send(command);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetLoyaltyAccount), new { customerId }, result.Value)
                : HandleError(result.Error);
        }

        [HttpPost("points")]
        public async Task<IActionResult> AwardPoints(
            Guid customerId,
            [FromBody] AwardLoyaltyPointsCommand command)
        {
            if (customerId != command.CustomerId)
                return BadRequest("Customer ID in URL does not match the command.");

            var result = await _sender.Send(command);

            return result.IsSuccess ? NoContent() : HandleError(result.Error);
        }

        [HttpPost("redemptions")]
        public async Task<IActionResult> RedeemPoints(
            Guid customerId,
            [FromBody] RedeemLoyaltyPointsCommand command)
        {
            if (customerId != command.CustomerId)
                return BadRequest("Customer ID in URL does not match the command.");

            var result = await _sender.Send(command);

            return result.IsSuccess ? Ok(result.Value) : HandleError(result.Error);
        }
    }
}

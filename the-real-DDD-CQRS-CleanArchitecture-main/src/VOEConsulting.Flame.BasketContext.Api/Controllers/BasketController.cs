using MediatR;
using Microsoft.AspNetCore.Mvc;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.AddItemToBasket;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.CreateBasket;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Queries.GetBasket;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.CalculateTotalAmount;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.ClearBasket;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.DeleteBasketItem;
using VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.UpdateBasketItem;

namespace VOEConsulting.Flame.BasketContext.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController(ISender sender, ILogger<BasketController> logger) : BaseController(logger)
    {
        private readonly ISender _sender = sender;

        // GET api/basket/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasket(Guid id)
        {
            var result = await _sender.Send(new GetBasketQuery(id));
            return result.IsSuccess ? Ok(result) : HandleError(result.Error);
        }

        // POST api/basket
        [HttpPost]
        public async Task<IActionResult> CreateBasket([FromBody] CreateBasketCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetBasket), new { id = result.Value }, result.Value);
            return HandleError(result.Error);
        }

        // POST api/basket/{id}/items
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItemToBasket(Guid id, [FromBody] AddItemToBasketCommand command)
        {
            if (id != command.BasketId)
                return BadRequest("Basket ID in URL does not match the command.");

           var result = await _sender.Send(command);
            if(result.IsSuccess)
            return NoContent();
            else
                return HandleError(result.Error);
        }

        [HttpPut("{id}/items/{itemId}")]
        public async Task<IActionResult> UpdateBasketItemCount(Guid id, Guid itemId, [FromBody] UpdateItemCountRequest request)
        {
            var result = await _sender.Send(new UpdateBasketItemCountCommand(id, itemId, request.Quantity));
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> DeleteBasketItem(Guid id, Guid itemId)
        {
            var result = await _sender.Send(new DeleteBasketItemCommand(id, itemId));
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpDelete("{id}/items")]
        public async Task<IActionResult> ClearBasket(Guid id)
        {
            var result = await _sender.Send(new ClearBasketCommand(id));
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpPost("{id}/calculate-total")]
        public async Task<IActionResult> CalculateTotalAmount(Guid id)
        {
            var result = await _sender.Send(new CalculateTotalAmountCommand(id));
            if (result.IsSuccess)
                return Ok(result.Value);
            return HandleError(result.Error);
        }

        [HttpGet("/test")]
        public string Test()
        {
            return "hello";
        }
    }

    public record UpdateItemCountRequest(int Quantity);

}

using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.ClearBasket
{
    public record ClearBasketCommand(Guid BasketId) : ICommand<Guid>;

}

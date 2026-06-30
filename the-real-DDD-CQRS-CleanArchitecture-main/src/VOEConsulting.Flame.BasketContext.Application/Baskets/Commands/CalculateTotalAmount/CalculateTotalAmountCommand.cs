using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Baskets.Commands.CalculateTotalAmount
{
    public record CalculateTotalAmountCommand(Guid BasketId) : ICommand<decimal>;

}

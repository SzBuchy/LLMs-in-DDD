using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.CreateLoyaltyAccount
{
    public record CreateLoyaltyAccountCommand(Guid CustomerId) : ICommand<Guid>;
}

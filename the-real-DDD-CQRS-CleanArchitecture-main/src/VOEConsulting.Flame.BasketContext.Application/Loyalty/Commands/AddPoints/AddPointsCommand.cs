using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.AddPoints
{
    public record AddPointsCommand(Guid CustomerId, int Amount, DateTimeOffset? EarnedAtUtc = null) : ICommand<Guid>;
}

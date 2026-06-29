using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Commands.RedeemPoints
{
    public record RedeemPointsCommand(Guid CustomerId, int Points, DateTimeOffset? RedeemedAtUtc = null) : ICommand<Guid>;
}

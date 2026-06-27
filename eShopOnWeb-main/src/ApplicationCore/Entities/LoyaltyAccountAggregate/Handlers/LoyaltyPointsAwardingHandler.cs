using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate.Events;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate.Handlers;

// Awards loyalty points whenever an order is placed: 1 point per whole currency unit spent.
public class LoyaltyPointsAwardingHandler(ILoyaltyAccountService loyaltyAccountService) : INotificationHandler<OrderCreatedEvent>
{
    public const int PointsPerCurrencyUnitSpent = 1;

    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var pointsEarned = (int)Math.Floor(domainEvent.Order.Total()) * PointsPerCurrencyUnitSpent;
        if (pointsEarned > 0)
        {
            await loyaltyAccountService.EarnPointsAsync(domainEvent.Order.BuyerId, pointsEarned);
        }
    }
}

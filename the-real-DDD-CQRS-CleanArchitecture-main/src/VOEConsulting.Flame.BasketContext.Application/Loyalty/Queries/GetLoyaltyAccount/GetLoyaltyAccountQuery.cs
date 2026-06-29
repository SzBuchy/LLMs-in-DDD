using System;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount
{
    public record GetLoyaltyAccountQuery(Guid CustomerId) : IQuery<LoyaltyAccountDto>;
}

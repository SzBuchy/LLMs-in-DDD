using System;
using System.Collections.Generic;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos
{
    public record LoyaltyAccountDto
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public int AvailablePointsBalance { get; init; }
        public List<LoyaltyPointsEntryDto> PointsEntries { get; init; } = new();
    }
}

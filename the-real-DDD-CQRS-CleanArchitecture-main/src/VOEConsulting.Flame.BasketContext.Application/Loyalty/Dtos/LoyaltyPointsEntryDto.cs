using System;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos
{
    public record LoyaltyPointsEntryDto
    {
        public Guid Id { get; init; }
        public int Amount { get; init; }
        public int UsedAmount { get; init; }
        public DateTimeOffset EarnedAtUtc { get; init; }
        public DateTimeOffset ExpiresAtUtc { get; init; }
        public bool IsExpired { get; init; }
    }
}

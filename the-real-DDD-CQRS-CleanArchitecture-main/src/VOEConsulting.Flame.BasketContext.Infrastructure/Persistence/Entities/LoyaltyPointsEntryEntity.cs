using System;

namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyPointsEntryEntity
    {
        public Guid Id { get; set; }
        public Guid LoyaltyAccountId { get; set; }
        public int Amount { get; set; }
        public int UsedAmount { get; set; }
        public DateTimeOffset EarnedAtUtc { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
    }
}

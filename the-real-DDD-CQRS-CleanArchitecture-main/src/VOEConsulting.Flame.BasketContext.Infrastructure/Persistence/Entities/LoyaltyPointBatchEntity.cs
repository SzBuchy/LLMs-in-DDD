namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyPointBatchEntity
    {
        public Guid Id { get; set; }
        public Guid LoyaltyAccountId { get; set; }
        public LoyaltyAccountEntity LoyaltyAccount { get; set; }
        public Guid OrderId { get; set; }
        public int Points { get; set; }
        public int RedeemedPoints { get; set; }
        public DateTimeOffset AwardedAtUtc { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public DateTimeOffset? ExpiredAtUtc { get; set; }
    }
}

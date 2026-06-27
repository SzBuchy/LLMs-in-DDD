namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyRedemptionEntity
    {
        public Guid Id { get; set; }
        public Guid LoyaltyAccountId { get; set; }
        public LoyaltyAccountEntity LoyaltyAccount { get; set; }
        public Guid OrderId { get; set; }
        public int Points { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTimeOffset RedeemedAtUtc { get; set; }
    }
}

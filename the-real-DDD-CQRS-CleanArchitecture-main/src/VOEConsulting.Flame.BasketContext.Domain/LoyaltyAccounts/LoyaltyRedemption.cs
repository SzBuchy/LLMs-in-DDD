namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts
{
    public sealed class LoyaltyRedemption : Entity<LoyaltyRedemption>
    {
        private LoyaltyRedemption(
            Id<LoyaltyRedemption> id,
            Guid orderId,
            int points,
            decimal discountAmount,
            DateTimeOffset redeemedAtUtc)
            : base(id)
        {
            OrderId = orderId.EnsureNotDefault();
            Points = points.EnsurePositive();
            DiscountAmount = discountAmount.EnsurePositive();
            RedeemedAtUtc = redeemedAtUtc;
        }

        public Guid OrderId { get; }
        public int Points { get; }
        public decimal DiscountAmount { get; }
        public DateTimeOffset RedeemedAtUtc { get; }

        public static LoyaltyRedemption Create(Guid orderId, int points, decimal discountAmount, DateTimeOffset redeemedAtUtc)
        {
            return new LoyaltyRedemption(Id<LoyaltyRedemption>.New(), orderId, points, discountAmount, redeemedAtUtc);
        }

        public static LoyaltyRedemption Restore(
            Id<LoyaltyRedemption> id,
            Guid orderId,
            int points,
            decimal discountAmount,
            DateTimeOffset redeemedAtUtc)
        {
            return new LoyaltyRedemption(id, orderId, points, discountAmount, redeemedAtUtc);
        }
    }
}

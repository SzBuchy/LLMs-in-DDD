namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    // Owned type (no identity of its own) - mirrors the LoyaltyPointsLot value object.
    public class LoyaltyPointsLotEntity
    {
        public int Points { get; set; }
        public DateTimeOffset EarnedAtUtc { get; set; }
    }
}

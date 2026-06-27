namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyAccountEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int MaxPointsPerRedemption { get; set; }
        public ICollection<LoyaltyPointBatchEntity> PointBatches { get; set; } = [];
        public ICollection<LoyaltyRedemptionEntity> Redemptions { get; set; } = [];
    }
}

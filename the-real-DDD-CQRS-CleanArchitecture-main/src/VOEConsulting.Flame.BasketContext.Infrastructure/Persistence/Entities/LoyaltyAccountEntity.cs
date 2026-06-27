namespace VOEConsulting.Flame.BasketContext.Infrastructure.Entities
{
    public class LoyaltyAccountEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public List<LoyaltyPointsLotEntity> PointsLots { get; set; } = [];
    }
}

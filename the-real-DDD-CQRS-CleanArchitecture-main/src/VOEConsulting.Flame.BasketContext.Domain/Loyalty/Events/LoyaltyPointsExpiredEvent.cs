namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class LoyaltyPointsExpiredEvent : BaseLoyaltyAccountDomainEvent
    {
        public LoyaltyPointsExpiredEvent(Id<LoyaltyAccount> accountId, int expiredPoints) : base(accountId)
        {
            ExpiredPoints = expiredPoints;
        }

        public int ExpiredPoints { get; }
    }
}

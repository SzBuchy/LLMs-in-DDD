namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class LoyaltyPointsEarnedEvent : BaseLoyaltyAccountDomainEvent
    {
        public LoyaltyPointsEarnedEvent(Id<LoyaltyAccount> accountId, int points) : base(accountId)
        {
            Points = points;
        }

        public int Points { get; }
    }
}

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class LoyaltyPointsRedeemedEvent : BaseLoyaltyAccountDomainEvent
    {
        public LoyaltyPointsRedeemedEvent(Id<LoyaltyAccount> accountId, int points, decimal discountAmount) : base(accountId)
        {
            Points = points;
            DiscountAmount = discountAmount;
        }

        public int Points { get; }
        public decimal DiscountAmount { get; }
    }
}

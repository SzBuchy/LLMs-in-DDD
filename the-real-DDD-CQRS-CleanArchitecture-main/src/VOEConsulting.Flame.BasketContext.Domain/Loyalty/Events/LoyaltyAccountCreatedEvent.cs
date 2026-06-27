namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class LoyaltyAccountCreatedEvent : BaseLoyaltyAccountDomainEvent
    {
        public LoyaltyAccountCreatedEvent(Id<LoyaltyAccount> accountId) : base(accountId) { }
    }
}

using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class PointsAddedEvent : BaseLoyaltyDomainEvent
    {
        public Id<Customer> CustomerId { get; }
        public int Amount { get; }
        public DateTimeOffset EarnedAtUtc { get; }

        public PointsAddedEvent(Id<LoyaltyAccount> loyaltyAccountId, Id<Customer> customerId, int amount, DateTimeOffset earnedAtUtc) 
            : base(loyaltyAccountId)
        {
            CustomerId = customerId;
            Amount = amount;
            EarnedAtUtc = earnedAtUtc;
        }
    }
}

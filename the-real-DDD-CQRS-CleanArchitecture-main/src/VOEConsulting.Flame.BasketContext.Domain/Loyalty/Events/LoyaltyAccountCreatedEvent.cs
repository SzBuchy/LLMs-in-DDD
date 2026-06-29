using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class LoyaltyAccountCreatedEvent : BaseLoyaltyDomainEvent
    {
        public Id<Customer> CustomerId { get; }

        public LoyaltyAccountCreatedEvent(Id<LoyaltyAccount> loyaltyAccountId, Id<Customer> customerId) 
            : base(loyaltyAccountId)
        {
            CustomerId = customerId;
        }
    }
}

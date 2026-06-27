using VOEConsulting.Flame.BasketContext.Domain.Baskets;

namespace VOEConsulting.Flame.BasketContext.Domain.LoyaltyAccounts.Events
{
    public sealed class LoyaltyPointsRedeemedEvent(
        Id<LoyaltyAccount> aggregateId,
        Id<Customer> customerId,
        Guid orderId,
        int points,
        decimal discountAmount)
        : BaseLoyaltyAccountDomainEvent(aggregateId)
    {
        public Id<Customer> CustomerId { get; } = customerId;
        public Guid OrderId { get; } = orderId;
        public int Points { get; } = points;
        public decimal DiscountAmount { get; } = discountAmount;
    }
}

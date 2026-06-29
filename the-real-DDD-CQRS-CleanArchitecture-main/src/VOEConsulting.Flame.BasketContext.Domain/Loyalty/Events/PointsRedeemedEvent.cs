using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.Common.Domain;

namespace VOEConsulting.Flame.BasketContext.Domain.Loyalty.Events
{
    public sealed class PointsRedeemedEvent : BaseLoyaltyDomainEvent
    {
        public Id<Customer> CustomerId { get; }
        public int PointsRedeemed { get; }
        public DateTimeOffset RedeemedAtUtc { get; }

        public PointsRedeemedEvent(Id<LoyaltyAccount> loyaltyAccountId, Id<Customer> customerId, int pointsRedeemed, DateTimeOffset redeemedAtUtc) 
            : base(loyaltyAccountId)
        {
            CustomerId = customerId;
            PointsRedeemed = pointsRedeemed;
            RedeemedAtUtc = redeemedAtUtc;
        }
    }
}

using VOEConsulting.Flame.Common.Core.Events;

namespace VOEConsulting.Flame.BasketContext.Application.Events.Integration
{
    public sealed class BasketCreatedIntegrationEvent : IntegrationEvent
    {
        public BasketCreatedIntegrationEvent(Guid basketId, Guid customerId)
            : base(basketId)
        {
            CustomerId = customerId;
        }
        public BasketCreatedIntegrationEvent() { }

        public Guid CustomerId { get; set; }
    }
}

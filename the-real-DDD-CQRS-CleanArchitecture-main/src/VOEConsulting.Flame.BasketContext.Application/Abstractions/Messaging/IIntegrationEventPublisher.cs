using VOEConsulting.Flame.Common.Core.Events;

namespace VOEConsulting.Flame.BasketContext.Application.Abstractions.Messaging
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : IntegrationEvent;
    }

    public interface IIntegrationEventHandler<TEvent> where TEvent : IntegrationEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}

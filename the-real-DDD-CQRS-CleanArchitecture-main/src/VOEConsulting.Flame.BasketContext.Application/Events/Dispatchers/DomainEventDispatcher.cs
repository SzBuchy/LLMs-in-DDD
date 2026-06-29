using Microsoft.Extensions.DependencyInjection;
using VOEConsulting.Flame.Common.Domain.Events;

namespace VOEConsulting.Flame.BasketContext.Application.Events.Dispatchers
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            var eventQueue = new Queue<IDomainEvent>(events);

            while (eventQueue.Count > 0)
            {
                var domainEvent = eventQueue.Dequeue();
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var handleMethod = handlerType.GetMethod("Handle");
                    if (handleMethod == null) continue;

                    await (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken });
                }
            }
        }
    }
}

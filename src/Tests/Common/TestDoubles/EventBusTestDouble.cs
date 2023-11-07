using Bazaar.BuildingBlocks.EventBus.Abstractions;
using Bazaar.BuildingBlocks.EventBus.Events;

namespace TestingCommon.TestDoubles;

public class EventBusTestDouble : IEventBus
{
    private readonly List<IntegrationEvent> _publishedEvents = new();
    private readonly List<IIntegrationEventHandler> _subscribedHandlers = new();

    public bool PublishedAnyEvent => _publishedEvents.Any();

    public T? GetEvent<T>() where T : IntegrationEvent
    {
        return _publishedEvents.FirstOrDefault(e => e is T) as T;
    }

    public IEnumerable<T> GetEvents<T>() where T : IntegrationEvent
    {
        return _publishedEvents.Where(e => e is T)
            .Select(e => (T)e);
    }

    public void Publish(IntegrationEvent @event)
    {
        _publishedEvents.Add(@event);
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        throw new NotImplementedException();
    }
}

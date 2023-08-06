namespace Bazaar.Ordering.EventHandling;

public class OrderStocksConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStocksConfirmedIntegrationEvent>
{
    private readonly IEventBus _eventBus;

    public OrderStocksConfirmedIntegrationEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task Handle(OrderStocksConfirmedIntegrationEvent @event)
    {
        _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(@event.orderId));
        await Task.CompletedTask;
    }
}

namespace Bazaar.Ordering.Domain.EventHandling;

public class OrderStocksConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStocksConfirmedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStocksConfirmedIntegrationEventHandler> _logger;

    public OrderStocksConfirmedIntegrationEventHandler(
        IOrderRepository orderRepo, IEventBus eventBus,
        ILogger<OrderStocksConfirmedIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepo;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderStocksConfirmedIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning($"[StocksConfirmed] Event handler: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        order.StartPayment();
        _orderRepo.Update(order);
        _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(@event.OrderId));
    }
}

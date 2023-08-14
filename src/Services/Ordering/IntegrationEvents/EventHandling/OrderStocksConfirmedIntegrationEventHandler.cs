namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class OrderStocksConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStocksConfirmedIntegrationEvent>
{
    private readonly OrderingDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStocksConfirmedIntegrationEventHandler> _logger;

    public OrderStocksConfirmedIntegrationEventHandler(
        OrderingDbContext context, IEventBus eventBus, ILogger<OrderStocksConfirmedIntegrationEventHandler> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderStocksConfirmedIntegrationEvent @event)
    {
        var order = _context.Orders.Find(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning($"[StocksConfirmed] Event handler: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        order.Status = OrderStatus.ProcessingPayment;
        await _context.SaveChangesAsync();

        _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(@event.OrderId));
    }
}

namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class OrderItemsUnavailableIntegrationEventHandler : IIntegrationEventHandler<OrderItemsUnavailableIntegrationEvent>
{
    private readonly OrderingDbContext _context;
    private readonly ILogger<OrderItemsUnavailableIntegrationEvent> _logger;

    public OrderItemsUnavailableIntegrationEventHandler(OrderingDbContext context, ILogger<OrderItemsUnavailableIntegrationEvent> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(OrderItemsUnavailableIntegrationEvent @event)
    {
        var order = _context.Orders.Find(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning($"[ItemsUnavailableEvent] Event handler: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        _logger.LogCritical($"[ItemsUnavailableEvent] Event handler: Order {order.Id} has been removed from database. " +
            $"Items reported unavailable: {string.Join(", ", @event.UnavailableProductIds)}");
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}

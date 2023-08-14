namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class OrderStocksInadequateIntegrationEventHandler : IIntegrationEventHandler<OrderStocksInadequateIntegrationEvent>
{
    private readonly OrderingDbContext _context;
    private readonly ILogger<OrderStocksInadequateIntegrationEventHandler> _logger;

    public OrderStocksInadequateIntegrationEventHandler(OrderingDbContext context, ILogger<OrderStocksInadequateIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(OrderStocksInadequateIntegrationEvent @event)
    {
        var order = _context.Orders.Find(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning($"Event handler for [StocksInadequateEvent]: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        _logger.LogCritical($"Event handler for [StocksInadequateEvent]: Order {order.Id} has been removed from database. " +
            $"Items reported with inadequate stocks: {string.Join(", ", @event.OrderStockInadequateItems.Select(i => i.ProductId))}");
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}

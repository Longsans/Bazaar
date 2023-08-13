namespace Bazaar.Catalog.IntegrationEventHandling;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly CatalogDbContext _context;
    private readonly IEventBus _eventBus;

    public OrderCreatedIntegrationEventHandler(CatalogDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        var unavailableItems = new List<string>();
        var stockInadequateItems = new List<OrderStockInadequateItem>();

        foreach (var stockItem in @event.OrderStockItems)
        {
            var catalogItem = _context.CatalogItems.FirstOrDefault(i => i.ProductId == stockItem.ProductId);
            if (catalogItem == null)
            {
                unavailableItems.Add(stockItem.ProductId);
                continue;
            }

            if (catalogItem.AvailableStock < stockItem.Quantity)
            {
                stockInadequateItems.Add(new OrderStockInadequateItem { ProductId = stockItem.ProductId, });
                continue;
            }

            catalogItem.AvailableStock -= stockItem.Quantity;
        }

        if (unavailableItems.Any())
        {
            _context.RejectChanges();
            _eventBus.Publish(new OrderItemsUnavailableIntegrationEvent(@event.OrderId, unavailableItems));
        }
        else if (stockInadequateItems.Any())
        {
            _context.RejectChanges();
            _eventBus.Publish(new OrderStocksInadequateIntegrationEvent(@event.OrderId, stockInadequateItems));
        }
        else
        {
            await _context.SaveChangesAsync();
        }
    }
}

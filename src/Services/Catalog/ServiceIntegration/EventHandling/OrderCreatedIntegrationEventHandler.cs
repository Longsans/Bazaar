namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public OrderCreatedIntegrationEventHandler(ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        var unavailableItems = new List<string>();
        var stockInadequateItems = new List<OrderStockInadequateItem>();
        var stockUpdatesList = new List<Action>();

        foreach (var stockItem in @event.OrderStockItems)
        {
            var catalogItem = _catalogRepo.GetByProductId(stockItem.ProductId);
            if (catalogItem == null)
            {
                unavailableItems.Add(stockItem.ProductId);
                continue;
            }

            if (stockItem.Quantity == 0)
            {
                continue;
            }

            if (catalogItem.AvailableStock < stockItem.Quantity)
            {
                stockInadequateItems.Add(
                    new OrderStockInadequateItem { ProductId = stockItem.ProductId, });
                continue;
            }

            stockUpdatesList.Add(() =>
            {
                catalogItem.ReduceStock(stockItem.Quantity);
                _catalogRepo.Update(catalogItem);
            });
        }

        if (unavailableItems.Any())
        {
            _eventBus.Publish(
                new OrderItemsUnavailableIntegrationEvent(
                    @event.OrderId, unavailableItems));
        }
        else if (stockInadequateItems.Any())
        {
            _eventBus.Publish(
                new OrderStocksInadequateIntegrationEvent(
                    @event.OrderId, stockInadequateItems));
        }
        else if (stockUpdatesList.Any())
        {
            foreach (var updateStock in stockUpdatesList)
            {
                updateStock();
            }

            _eventBus.Publish(
                new OrderStocksConfirmedIntegrationEvent(@event.OrderId));
        }

        await Task.CompletedTask;
    }
}

namespace Bazaar.Catalog.Application.EventHandling;

public class OrderPlacedIntegrationEventHandler
    : IIntegrationEventHandler<OrderPlacedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly IEventBus _eventBus;

    public OrderPlacedIntegrationEventHandler(IRepository<CatalogItem> catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(OrderPlacedIntegrationEvent @event)
    {
        var stockRejectedItems = new List<StockRejectedOrderItem>();
        var nonFbbItems = new List<CatalogItem>();
        var fbbItems = new List<OrderedFbbProduct>();

        foreach (var orderItem in @event.OrderItems)
        {
            var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
                new CatalogItemByProductIdSpec(orderItem.ProductId));
            if (catalogItem == null)
            {
                stockRejectedItems.Add(new(
                    orderItem.ProductId,
                    orderItem.Quantity,
                    null, StockRejectionReason.NoListing));
                continue;
            }

            if (orderItem.Quantity == 0)
            {
                continue;
            }
            if (catalogItem.AvailableStock < orderItem.Quantity)
            {
                stockRejectedItems.Add(new(
                    orderItem.ProductId,
                    orderItem.Quantity,
                    catalogItem.AvailableStock,
                    StockRejectionReason.InsufficientStock));
                continue;
            }

            if (stockRejectedItems.Any())
            {
                continue;
            }
            if (catalogItem.IsFbb)
            {
                fbbItems.Add(new OrderedFbbProduct(
                    catalogItem.ProductId, orderItem.Quantity));
            }
            else
            {
                catalogItem.ReduceStock(orderItem.Quantity);
                nonFbbItems.Add(catalogItem);
            }
        }

        if (stockRejectedItems.Any())
        {
            _eventBus.Publish(new OrderItemsStockRejectedIntegrationEvent(
                @event.OrderId, stockRejectedItems));
            return;
        }

        if (nonFbbItems.Any())
        {
            await _catalogRepo.UpdateRangeAsync(nonFbbItems);
            _eventBus.Publish(new OrderItemsStockConfirmedIntegrationEvent(
                @event.OrderId, nonFbbItems.Select(x => x.ProductId)));
        }
        if (fbbItems.Any())
        {
            _eventBus.Publish(new FbbProductsOrderedIntegrationEvent(@event.OrderId, fbbItems));
        }

        await Task.CompletedTask;
    }
}

namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class BasketCheckoutAcceptedIntegrationEventHandler
    : IIntegrationEventHandler<BasketCheckoutAcceptedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public BasketCheckoutAcceptedIntegrationEventHandler(ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(BasketCheckoutAcceptedIntegrationEvent @event)
    {
        var unavailableItems = new List<string>();
        var stockInadequateItems = new List<CheckoutItemWithoutEnoughStock>();
        var stockUpdatesList = new List<Action>();

        foreach (var stockItem in @event.BasketItems)
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
                    new CheckoutItemWithoutEnoughStock
                    {
                        ProductId = stockItem.ProductId,
                        PurchaseQuantity = stockItem.Quantity,
                        AvailableStock = catalogItem.AvailableStock
                    });
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
                new BasketCheckoutItemsUnavailableIntegrationEvent(
                    @event.BuyerId, unavailableItems));
        }
        else if (stockInadequateItems.Any())
        {
            _eventBus.Publish(
                new BasketCheckoutNotEnoughStocksIntegrationEvent(
                    @event.BuyerId, stockInadequateItems));
        }
        else if (stockUpdatesList.Any())
        {
            foreach (var updateStock in stockUpdatesList)
            {
                updateStock();
            }

            _eventBus.Publish(
                new BasketCheckoutStocksConfirmedIntegrationEvent(@event.BuyerId));
        }

        await Task.CompletedTask;
    }
}

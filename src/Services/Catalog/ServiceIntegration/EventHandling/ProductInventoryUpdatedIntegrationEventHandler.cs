namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class ProductInventoryUpdatedIntegrationEventHandler
    : IIntegrationEventHandler<ProductInventoryUpdatedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public ProductInventoryUpdatedIntegrationEventHandler(
        ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductInventoryUpdatedIntegrationEvent @event)
    {
        var catalogItem = _catalogRepo.GetByProductId(@event.ProductId);
        if (catalogItem == null)
        {
            _eventBus.Publish(new CatalogItemDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        if (@event.UpdatedStock == catalogItem.AvailableStock)
        {
            return;
        }

        if (@event.UpdatedStock < catalogItem.AvailableStock)
        {
            catalogItem.ReduceStock(catalogItem.AvailableStock - @event.UpdatedStock);
        }
        else
        {
            catalogItem.Restock(@event.UpdatedStock - catalogItem.AvailableStock);
        }

        _catalogRepo.Update(catalogItem);
        await Task.CompletedTask;
    }
}

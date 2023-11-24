﻿namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class ProductFbbInventoryUpdatedIntegrationEventHandler
    : IIntegrationEventHandler<ProductFbbInventoryUpdatedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public ProductFbbInventoryUpdatedIntegrationEventHandler(
        ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductFbbInventoryUpdatedIntegrationEvent @event)
    {
        var catalogItem = _catalogRepo.GetByProductId(@event.ProductId);
        if (catalogItem == null)
        {
            _eventBus.Publish(new CatalogItemDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        if (@event.FulfillableStock == catalogItem.AvailableStock)
        {
            return;
        }

        if (@event.FulfillableStock < catalogItem.AvailableStock)
        {
            catalogItem.ReduceStock(catalogItem.AvailableStock - @event.FulfillableStock);
        }
        else
        {
            catalogItem.Restock(@event.FulfillableStock - catalogItem.AvailableStock);
        }

        _catalogRepo.Update(catalogItem);
        await Task.CompletedTask;
    }
}

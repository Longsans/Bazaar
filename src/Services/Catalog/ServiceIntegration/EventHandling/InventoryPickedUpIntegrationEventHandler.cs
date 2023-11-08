namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class InventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<InventoryPickedUpIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;

    public InventoryPickedUpIntegrationEventHandler(ICatalogRepository catalogRepo)
    {
        _catalogRepo = catalogRepo;
    }

    public async Task Handle(InventoryPickedUpIntegrationEvent @event)
    {
        foreach (var inventoryItem in @event.Inventories)
        {
            var catalogItem = _catalogRepo.GetByProductId(inventoryItem.ProductId);
            if (catalogItem == null)
            {
                return;
            }
            catalogItem.UpdateOfficiallyListed(true);
            _catalogRepo.Update(catalogItem);
        }
        await Task.CompletedTask;
    }
}

namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class FbbInventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<FbbInventoryPickedUpIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;

    public FbbInventoryPickedUpIntegrationEventHandler(ICatalogRepository catalogRepo)
    {
        _catalogRepo = catalogRepo;
    }

    public async Task Handle(FbbInventoryPickedUpIntegrationEvent @event)
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

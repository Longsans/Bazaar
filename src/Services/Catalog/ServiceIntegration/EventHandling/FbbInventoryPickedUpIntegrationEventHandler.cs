namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class FbbInventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<FbbInventoryPickedUpIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly ILogger<FbbInventoryPickedUpIntegrationEventHandler> _logger;

    public FbbInventoryPickedUpIntegrationEventHandler(ICatalogRepository catalogRepo,
        ILogger<FbbInventoryPickedUpIntegrationEventHandler> logger)
    {
        _catalogRepo = catalogRepo;
        _logger = logger;
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
            try
            {
                catalogItem.ReactivateOutOfStockListing();
            }
            catch (InvalidOperationException)
            {
                _logger.LogError(
                    "Error reactivating stock listing - catalog item was not out of stock. " +
                    "Catalog item {id} has status: {status}", catalogItem.ProductId, catalogItem.ListingStatus);

                return;
            }
            _catalogRepo.Update(catalogItem);
        }
        await Task.CompletedTask;
    }
}

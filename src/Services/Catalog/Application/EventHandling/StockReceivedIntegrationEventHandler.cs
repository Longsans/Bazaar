namespace Bazaar.Catalog.Application.EventHandling;

public class StockReceivedIntegrationEventHandler : IIntegrationEventHandler<StockReceivedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly ILogger<StockReceivedIntegrationEventHandler> _logger;

    public StockReceivedIntegrationEventHandler(
        ICatalogRepository catalogRepo,
        ILogger<StockReceivedIntegrationEventHandler> logger)
    {
        _catalogRepo = catalogRepo;
        _logger = logger;
    }

    public async Task Handle(StockReceivedIntegrationEvent @event)
    {
        var updatedItems = new List<CatalogItem>();
        foreach (var receivedItem in @event.Items)
        {
            var catalogItem = _catalogRepo.GetByProductId(receivedItem.ProductId);
            if (catalogItem == null || catalogItem.IsDeleted)
            {
                _logger.LogInformation("Notified of stock received for deleted listing {ProductId}",
                    receivedItem.ProductId);
                continue;
            }

            try
            {
                catalogItem.Restock(receivedItem.GoodQuantity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating stock from received stock for catalog item {ProductId}: {Ex}",
                    catalogItem.ProductId, ex.Message);
                return;
            }
            updatedItems.Add(catalogItem);
        }
        _catalogRepo.UpdateRange(updatedItems);
        await Task.CompletedTask;
    }
}

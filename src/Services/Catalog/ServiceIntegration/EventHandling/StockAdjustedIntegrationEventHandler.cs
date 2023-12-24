namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class StockAdjustedIntegrationEventHandler : IIntegrationEventHandler<StockAdjustedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly ILogger<StockAdjustedIntegrationEventHandler> _logger;

    public StockAdjustedIntegrationEventHandler(
        ICatalogRepository catalogRepo,
        ILogger<StockAdjustedIntegrationEventHandler> logger)
    {
        _catalogRepo = catalogRepo;
        _logger = logger;
    }

    public async Task Handle(StockAdjustedIntegrationEvent @event)
    {
        var updatedItems = new List<CatalogItem>();
        foreach (var quantityAdjusted in @event.QuantitiesAdjusted)
        {
            var catalogItem = _catalogRepo.GetByProductId(quantityAdjusted.ProductId);
            if (catalogItem == null || catalogItem.IsDeleted)
            {
                _logger.LogInformation("Notified of stock adjusted for deleted listing {ProductId}",
                    quantityAdjusted.ProductId);
                continue;
            }

            var unsignedQuantity = (uint)Math.Abs(quantityAdjusted.GoodQuantity);
            try
            {
                if (quantityAdjusted.GoodQuantity > 0)
                {
                    catalogItem.Restock(unsignedQuantity);
                }
                else
                {
                    catalogItem.ReduceStock(unsignedQuantity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating stock from adjustment for catalog item {ProductId}: {Ex}",
                    catalogItem.ProductId, ex.Message);
                return;
            }
            updatedItems.Add(catalogItem);
        }
        _catalogRepo.UpdateRange(updatedItems);
        await Task.CompletedTask;
    }
}

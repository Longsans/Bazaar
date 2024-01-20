namespace Bazaar.Catalog.Application.EventHandling;

public class ProductImageSavedIntegrationEventHandler : IIntegrationEventHandler<ProductImageSavedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly ILogger<ProductImageSavedIntegrationEventHandler> _logger;

    public ProductImageSavedIntegrationEventHandler(
        IRepository<CatalogItem> catalogRepo, ILogger<ProductImageSavedIntegrationEventHandler> logger)
    {
        _catalogRepo = catalogRepo;
        _logger = logger;
    }

    public async Task Handle(ProductImageSavedIntegrationEvent @event)
    {
        var catalogItem = await _catalogRepo.FirstOrDefaultAsync(new CatalogItemByProductIdSpec(@event.ProductId));
        if (catalogItem is null)
        {
            _logger.LogError("Error handling image saved event: " +
                "Catalog item with product ID {productId} does not exist or has been deleted.", @event.ProductId);
            return;
        }
        var imageFilename = Path.GetFileName(new Uri(@event.ImageUrl).LocalPath);
        catalogItem.ChangeProductDetails(imageFilename: imageFilename);
        await _catalogRepo.UpdateAsync(catalogItem);
    }
}

namespace Bazaar.FbbInventory.Application.EventHandling;

public class CatalogItemCreatedIntegrationEventHandler : IIntegrationEventHandler<CatalogItemCreatedIntegrationEvent>
{
    private readonly IRepository<SellerInventory> _sellerInventories;
    private readonly ILogger<CatalogItemCreatedIntegrationEventHandler> _logger;

    public CatalogItemCreatedIntegrationEventHandler(IRepository<SellerInventory> sellerInventories, ILogger<CatalogItemCreatedIntegrationEventHandler> logger)
    {
        _sellerInventories = sellerInventories;
        _logger = logger;
    }

    public async Task Handle(CatalogItemCreatedIntegrationEvent @event)
    {
        if (@event.FulfillmentMethod != FulfillmentMethod.Fbb)
        {
            _logger.LogInformation("Processed catalog item created event {eventId}: product {productId} is not FBB. No inventory was created.",
                @event.Id, @event.ProductId);
            return;
        }

        var sellerInventory = await _sellerInventories.SingleOrDefaultAsync(new SellerInventoryWithProductsAndLotsSpec(@event.SellerId));
        if (sellerInventory is null)
        {
            _logger.LogError("Error processing catalog item created event {eventId}: Seller inventory with seller ID {sellerId} not found.", @event.Id, @event.SellerId);
            return;
        }
        var productInventory = new ProductInventory(
            @event.ProductId, 0u, 0u, 0u, 0u, 100u,
            @event.ProductLengthCm, @event.ProductWidthCm, @event.ProductHeightCm, sellerInventory.Id);
        sellerInventory.ProductInventories.Add(productInventory);
        await _sellerInventories.UpdateAsync(sellerInventory);
        _logger.LogCritical("Succeeded in processing catalog item created event {eventId}. New inventory for product {productId} created, ID: {inventoryId}.",
            @event.Id, @event.ProductId, productInventory.Id);
    }
}

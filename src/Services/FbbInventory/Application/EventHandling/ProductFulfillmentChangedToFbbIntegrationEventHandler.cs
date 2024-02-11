namespace Bazaar.FbbInventory.Application.EventHandling;

public class ProductFulfillmentChangedToFbbIntegrationEventHandler : IIntegrationEventHandler<ProductFulfillmentChangedToFbbIntegrationEvent>
{
    private readonly IRepository<ProductInventory> _productInventories;
    private readonly IRepository<SellerInventory> _sellerInventories;
    private readonly ILogger<ProductFulfillmentChangedToFbbIntegrationEventHandler> _logger;

    public ProductFulfillmentChangedToFbbIntegrationEventHandler(
        IRepository<ProductInventory> productInventoryRepo,
        IRepository<SellerInventory> sellerInventoryRepo,
        ILogger<ProductFulfillmentChangedToFbbIntegrationEventHandler> logger)
    {
        _productInventories = productInventoryRepo;
        _sellerInventories = sellerInventoryRepo;
        _logger = logger;
    }

    public async Task Handle(ProductFulfillmentChangedToFbbIntegrationEvent @event)
    {
        var inventory = await _productInventories.SingleOrDefaultAsync(new ProductInventoryWithLotsAndSellerSpec(@event.ProductId));
        if (inventory is null)
        {
            var sellerInventory = await _sellerInventories.SingleOrDefaultAsync(
                new SellerInventoryWithProductsAndLotsSpec(@event.SellerId));
            if (sellerInventory is null)
            {
                _logger.LogError("Error processing fulfillment changed to FBB event: No product inventory nor seller inventory were found for product ID and seller ID.");
                return;
            }
            inventory = new ProductInventory(
                @event.ProductId, 0u, 0u, 0u, 0u, 100u,
                @event.ProductLengthInCm, @event.ProductWidthInCm, @event.ProductHeightInCm, sellerInventory.Id);
            sellerInventory.ProductInventories.Add(inventory);
            await _sellerInventories.UpdateAsync(sellerInventory);
            return;
        }

        try
        {
            inventory.ConfirmStrandingResolved();
            await _productInventories.UpdateAsync(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error resolving stranding for product {productId}: {error}.", @event.ProductId, ex.Message);
        }
    }
}

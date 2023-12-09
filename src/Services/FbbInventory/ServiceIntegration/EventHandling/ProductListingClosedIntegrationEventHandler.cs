namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductListingClosedIntegrationEventHandler
    : IIntegrationEventHandler<ProductListingClosedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public ProductListingClosedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(ProductListingClosedIntegrationEvent @event)
    {
        var productInventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (productInventory == null)
        {
            return;
        }

        productInventory.LabelAllFulfillableStockAsStrandedStock();
        _productInventoryRepo.Update(productInventory);
        await Task.CompletedTask;
    }
}

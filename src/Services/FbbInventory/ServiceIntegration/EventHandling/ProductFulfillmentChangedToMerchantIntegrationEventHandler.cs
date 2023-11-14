namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductFulfillmentChangedToMerchantIntegrationEventHandler
    : IIntegrationEventHandler<ProductFulfillmentChangedToMerchantIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public ProductFulfillmentChangedToMerchantIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(ProductFulfillmentChangedToMerchantIntegrationEvent @event)
    {
        var inventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (inventory == null)
        {
            return;
        }

        inventory.MarkAsUnfulfillable();
        _productInventoryRepo.Update(inventory);
        await Task.CompletedTask;
    }
}

namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductRelistedIntegrationEventHandler :
    IIntegrationEventHandler<ProductRelistedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public ProductRelistedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(ProductRelistedIntegrationEvent @event)
    {
        var productInven = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (productInven == null)
        {
            return;
        }

        productInven.RelabelStrandedStockAsFulfillableStock();
        _productInventoryRepo.Update(productInven);
        await Task.CompletedTask;
    }
}

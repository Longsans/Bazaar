namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductFulfillmentChangedToMerchantIntegrationEventHandler
    : IIntegrationEventHandler<ProductFulfillmentChangedToMerchantIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public ProductFulfillmentChangedToMerchantIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductFulfillmentChangedToMerchantIntegrationEvent @event)
    {
        var inventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (inventory == null)
        {
            _eventBus.Publish(new ProductFbbInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        var unitsInStock = inventory.FulfillableUnitsInStock;
        inventory.ReduceFulfillableStock(unitsInStock);
        inventory.AddUnfulfillableStock(UnfulfillableCategory.Stranded, unitsInStock);
        _productInventoryRepo.Update(inventory);
        await Task.CompletedTask;
    }
}

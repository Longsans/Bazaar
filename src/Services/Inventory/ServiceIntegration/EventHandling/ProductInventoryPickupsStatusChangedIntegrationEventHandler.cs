namespace Bazaar.Inventory.ServiceIntegration.EventHandling;

public class ProductInventoryPickupsStatusChangedIntegrationEventHandler
    : IIntegrationEventHandler<ProductInventoryPickupsStatusChangedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public ProductInventoryPickupsStatusChangedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductInventoryPickupsStatusChangedIntegrationEvent @event)
    {
        var productInventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (productInventory == null)
        {
            _eventBus.Publish(
                new ProductInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        productInventory.UpdateHasPickupsInProgress(
            @event.InProgressPickups != 0);
        _productInventoryRepo.Update(productInventory);
        await Task.CompletedTask;
    }
}

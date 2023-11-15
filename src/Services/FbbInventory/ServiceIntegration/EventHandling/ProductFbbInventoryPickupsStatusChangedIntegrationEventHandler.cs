namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler
    : IIntegrationEventHandler<ProductFbbInventoryPickupsStatusChangedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductFbbInventoryPickupsStatusChangedIntegrationEvent @event)
    {
        var productInventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (productInventory == null)
        {
            _eventBus.Publish(
                new ProductFbbInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        productInventory.UpdateHasPickupsInProgress(
            @event.InProgressPickups != 0);
        _productInventoryRepo.Update(productInventory);
        await Task.CompletedTask;
    }
}

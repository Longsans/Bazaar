namespace Bazaar.FbbInventory.Application.EventHandling;

public class ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler
    : IIntegrationEventHandler<ProductFbbInventoryPickupsStatusChangedIntegrationEvent>
{
    private readonly Repository<ProductInventory> _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler(
        Repository<ProductInventory> productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(ProductFbbInventoryPickupsStatusChangedIntegrationEvent @event)
    {
        var productInventory = await _productInventoryRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(@event.ProductId));
        if (productInventory == null)
        {
            _eventBus.Publish(
                new ProductFbbInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        productInventory.UpdateHasPickupsInProgress(
            @event.InProgressPickups != 0);
        await _productInventoryRepo.UpdateAsync(productInventory);
        await Task.CompletedTask;
    }
}

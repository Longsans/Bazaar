namespace Bazaar.Transport.ServiceIntegration.EventHandling;

public class ProductInventoryDeletedIntegrationEventHandler
    : IIntegrationEventHandler<ProductInventoryDeletedIntegrationEvent>
{
    private readonly IInventoryPickupRepository _pickupRepo;

    public ProductInventoryDeletedIntegrationEventHandler(
        IInventoryPickupRepository pickupRepo)
    {
        _pickupRepo = pickupRepo;
    }

    public async Task Handle(ProductInventoryDeletedIntegrationEvent @event)
    {
        var pickupsForProduct = _pickupRepo.GetByProductId(@event.ProductId)
            .Where(x => x.Status == InventoryPickupStatus.Scheduled)
            .ToList();
        foreach (var pickup in pickupsForProduct)
        {
            pickup.Cancel(CommonCancelReasons.ProductFbbInventoryCancelled);
            _pickupRepo.Update(pickup);
        }

        await Task.CompletedTask;
    }
}

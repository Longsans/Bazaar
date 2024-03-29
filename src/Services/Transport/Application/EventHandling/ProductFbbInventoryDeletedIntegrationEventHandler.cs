﻿namespace Bazaar.Transport.ServiceIntegration.EventHandling;

public class ProductFbbInventoryDeletedIntegrationEventHandler
    : IIntegrationEventHandler<ProductFbbInventoryDeletedIntegrationEvent>
{
    private readonly IRepository<InventoryPickup> _pickupRepo;

    public ProductFbbInventoryDeletedIntegrationEventHandler(
        IRepository<InventoryPickup> pickupRepo)
    {
        _pickupRepo = pickupRepo;
    }

    public async Task Handle(ProductFbbInventoryDeletedIntegrationEvent @event)
    {
        var pickupsForProduct = await _pickupRepo.ListAsync(
            new PickupsScheduledSpec(@event.ProductId));
        foreach (var pickup in pickupsForProduct)
        {
            pickup.Cancel(CommonCancelReasons.ProductFbbInventoryCancelled);
        }
        await _pickupRepo.UpdateRangeAsync(pickupsForProduct);
    }
}

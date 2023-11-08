namespace Bazaar.Inventory.ServiceIntegration.EventHandling;

public class InventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<InventoryPickedUpIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public InventoryPickedUpIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(InventoryPickedUpIntegrationEvent @event)
    {
        foreach (var pickupInventory in @event.Inventories)
        {
            var productInventory = _productInventoryRepo
                .GetByProductId(pickupInventory.ProductId);
            if (productInventory == null)
            {
                // This should never happen due to delete inventory saga
                return;
            }

            // This should not throw exceeding stock threshold
            productInventory.Restock(pickupInventory.StockUnits);
            _productInventoryRepo.Update(productInventory);
        }
        await Task.CompletedTask;
    }
}

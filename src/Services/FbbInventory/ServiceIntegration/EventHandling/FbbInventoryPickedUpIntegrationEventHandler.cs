namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class FbbInventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<FbbInventoryPickedUpIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public FbbInventoryPickedUpIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(FbbInventoryPickedUpIntegrationEvent @event)
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

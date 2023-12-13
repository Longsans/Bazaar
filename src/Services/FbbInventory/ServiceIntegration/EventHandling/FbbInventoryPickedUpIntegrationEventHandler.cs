namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class FbbInventoryPickedUpIntegrationEventHandler
    : IIntegrationEventHandler<FbbInventoryPickedUpIntegrationEvent>
{
    private readonly IUpdateProductStockService _updateStockService;

    public FbbInventoryPickedUpIntegrationEventHandler(IUpdateProductStockService updateStockService)
    {
        _updateStockService = updateStockService;
    }

    public async Task Handle(FbbInventoryPickedUpIntegrationEvent @event)
    {
        foreach (var pickupInventory in @event.Inventories)
        {
            // This should not throw exceeding stock threshold
            _updateStockService.AddFulfillableStock(
                pickupInventory.ProductId, pickupInventory.StockUnits);
        }
        await Task.CompletedTask;
    }
}

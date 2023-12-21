namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class InventoryReturnCompletedIntegrationEventHandler
    : IIntegrationEventHandler<InventoryReturnCompletedIntegrationEvent>
{
    private readonly ILotRepository _lotRepo;
    private readonly IProductInventoryRepository _productInventoryRepo;

    public InventoryReturnCompletedIntegrationEventHandler(ILotRepository lotRepo,
        IProductInventoryRepository productInventoryRepo)
    {
        _lotRepo = lotRepo;
        _productInventoryRepo = productInventoryRepo;
    }

    public async Task Handle(InventoryReturnCompletedIntegrationEvent @event)
    {
        var returnedInventories = new List<ProductInventory>();
        foreach (var returnedQuantity in @event.ReturnedLotQuantities)
        {
            var lot = _lotRepo.GetByLotNumber(returnedQuantity.LotNumber);
            if (lot == null)
            {
                // Should never be possible
                return;
            }

            lot.ConfirmUnitsRemoved(returnedQuantity.Quantity);
            var inventory = lot.ProductInventory;
            inventory.RemoveEmptyLots();
            returnedInventories.Add(inventory);
        }
        _productInventoryRepo.UpdateRange(returnedInventories);

        await Task.CompletedTask;
    }
}

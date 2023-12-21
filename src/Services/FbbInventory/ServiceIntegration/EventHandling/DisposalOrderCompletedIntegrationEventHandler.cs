namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class DisposalOrderCompletedIntegrationEventHandler
    : IIntegrationEventHandler<DisposalOrderCompletedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly ILotRepository _lotRepo;

    public DisposalOrderCompletedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo, ILotRepository lotRepo)
    {
        _productInventoryRepo = productInventoryRepo;
        _lotRepo = lotRepo;
    }

    public async Task Handle(DisposalOrderCompletedIntegrationEvent @event)
    {
        var restoredInventories = new List<ProductInventory>();
        foreach (var disposedQty in @event.DisposedQuantities)
        {
            var lot = _lotRepo.GetByLotNumber(disposedQty.LotNumber);
            if (lot == null)
            {
                // This should not be possible
                return;
            }

            lot.ConfirmUnitsRemoved(disposedQty.DisposedUnits);
            var inventory = lot.ProductInventory;
            inventory.RemoveEmptyLots();
            restoredInventories.Add(inventory);
        }
        _productInventoryRepo.UpdateRange(restoredInventories);

        await Task.CompletedTask;
    }
}

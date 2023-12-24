namespace Bazaar.FbbInventory.Application.EventHandling;

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
        var updatedInventories = new List<ProductInventory>();
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
            updatedInventories.Add(inventory);
        }

        foreach (var inventory in updatedInventories)
        {
            inventory.RemoveEmptyLots();
        }
        _productInventoryRepo.UpdateRange(updatedInventories);
        await Task.CompletedTask;
    }
}

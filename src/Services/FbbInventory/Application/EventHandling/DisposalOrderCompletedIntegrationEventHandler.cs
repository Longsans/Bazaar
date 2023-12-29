namespace Bazaar.FbbInventory.Application.EventHandling;

public class DisposalOrderCompletedIntegrationEventHandler
    : IIntegrationEventHandler<DisposalOrderCompletedIntegrationEvent>
{
    private readonly IRepository<ProductInventory> _productInventoryRepo;
    private readonly IRepository<Lot> _lotRepo;

    public DisposalOrderCompletedIntegrationEventHandler(
        IRepository<ProductInventory> productInventoryRepo, IRepository<Lot> lotRepo)
    {
        _productInventoryRepo = productInventoryRepo;
        _lotRepo = lotRepo;
    }

    public async Task Handle(DisposalOrderCompletedIntegrationEvent @event)
    {
        var updatedInventories = new List<ProductInventory>();
        foreach (var disposedQty in @event.DisposedQuantities)
        {
            var lot = await _lotRepo.SingleOrDefaultAsync(
                new LotWithInventoriesSpec(disposedQty.LotNumber));
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
        await _productInventoryRepo.UpdateRangeAsync(updatedInventories);
        await Task.CompletedTask;
    }
}

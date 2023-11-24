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
        foreach (var disposedQty in @event.DisposedQuantities)
        {
            var lot = _lotRepo.GetByLotNumber(disposedQty.LotNumber);
            if (lot == null)
            {
                // This should not be possible
                return;
            }

            var inventory = _productInventoryRepo.GetById(lot.ProductInventoryId);
            if (inventory == null)
            {
                return;
            }

            lot.RemovePendingUnits(disposedQty.DisposedUnits);
            inventory.RemoveEmptyLots();
            _productInventoryRepo.Update(inventory);
        }

        await Task.CompletedTask;
    }
}

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
        var lots = @event.LotsWithReturnedUnits.Select(x =>
        {
            var lot = _lotRepo.GetByLotNumber(x.LotNumber);
            lot?.RemovePendingUnits(x.Units);
            return lot;
        }).ToList();
        if (lots.Any(x => x == null))
        {
            return;
        }

        foreach (var lotGroup in lots.GroupBy(x => x.ProductInventoryId))
        {
            var inventory = _productInventoryRepo.GetById(lotGroup.Key)!;
            inventory.RemoveEmptyLots();
        }
        _lotRepo.UpdateRange(lots);

        await Task.CompletedTask;
    }
}

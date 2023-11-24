namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class InventoryReturnCancelledIntegrationEventHandler
    : IIntegrationEventHandler<InventoryReturnCancelledIntegrationEvent>
{
    private readonly ILotRepository _lotRepo;

    public InventoryReturnCancelledIntegrationEventHandler(ILotRepository lotRepo)
    {
        _lotRepo = lotRepo;
    }

    public async Task Handle(InventoryReturnCancelledIntegrationEvent @event)
    {
        var lots = @event.LotsWithUnreturnedUnits.Select(x =>
        {
            var lot = _lotRepo.GetByLotNumber(x.LotNumber);
            lot?.ReturnPendingUnitsToStock(x.Units);
            return lot;
        }).ToList();
        if (lots.Any(x => x == null))
        {
            return;
        }
        _lotRepo.UpdateRange(lots);

        await Task.CompletedTask;
    }
}

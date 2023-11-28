namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class DisposalOrderCancelledIntegrationEventHandler
    : IIntegrationEventHandler<DisposalOrderCancelledIntegrationEvent>
{
    private readonly ILotRepository _lotRepo;

    public DisposalOrderCancelledIntegrationEventHandler(
        ILotRepository lotRepo)
    {
        _lotRepo = lotRepo;
    }

    public async Task Handle(DisposalOrderCancelledIntegrationEvent @event)
    {
        foreach (var undisposedQty in @event.UndisposedQuantities)
        {
            var lot = _lotRepo.GetByLotNumber(undisposedQty.LotNumber);
            if (lot == null)
            {
                // Should not be possible
                return;
            }
            lot.ReturnPendingUnitsToStock(undisposedQty.UndisposedUnits);
            _lotRepo.Update(lot);
        }

        await Task.CompletedTask;
    }
}

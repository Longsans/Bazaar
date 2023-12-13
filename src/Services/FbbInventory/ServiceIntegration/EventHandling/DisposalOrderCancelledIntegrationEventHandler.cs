namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class DisposalOrderCancelledIntegrationEventHandler
    : IIntegrationEventHandler<DisposalOrderCancelledIntegrationEvent>
{
    private readonly ILotRepository _lotRepo;
    private readonly IEventBus _eventBus;

    public DisposalOrderCancelledIntegrationEventHandler(
        ILotRepository lotRepo, IEventBus eventBus)
    {
        _lotRepo = lotRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(DisposalOrderCancelledIntegrationEvent @event)
    {
        var affectedProductInventories = new List<ProductInventory>();
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

            if (!lot.IsUnfulfillable && !affectedProductInventories.Contains(lot.ProductInventory))
                affectedProductInventories.Add(lot.ProductInventory);
        }

        foreach (var inventory in affectedProductInventories)
        {
            _eventBus.Publish(new ProductFbbInventoryUpdatedIntegrationEvent(inventory));
        }

        await Task.CompletedTask;
    }
}

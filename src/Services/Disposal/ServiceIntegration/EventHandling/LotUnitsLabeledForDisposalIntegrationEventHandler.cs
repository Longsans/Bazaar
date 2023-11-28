namespace Bazaar.Disposal.ServiceIntegration.EventHandling;

public class LotUnitsLabeledForDisposalIntegrationEventHandler(
    IDisposalOrderRepository disposalOrderRepo)
        : IIntegrationEventHandler<LotUnitsLabeledForDisposalIntegrationEvent>
{
    private readonly IDisposalOrderRepository _disposalOrderRepo = disposalOrderRepo;

    public async Task Handle(LotUnitsLabeledForDisposalIntegrationEvent @event)
    {
        var disposeQuantities = @event.DisposeLotUnits
            .Select(x => new DisposeQuantity(x.LotNumber, x.UnitsToDispose, x.InventoryOwnerId));
        var disposalOrder = new DisposalOrder(disposeQuantities, @event.LabeledByBazaar);

        _disposalOrderRepo.Create(disposalOrder);
        await Task.CompletedTask;
    }
}

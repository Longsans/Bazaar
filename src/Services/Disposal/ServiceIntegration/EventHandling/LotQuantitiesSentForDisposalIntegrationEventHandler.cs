namespace Bazaar.Disposal.ServiceIntegration.EventHandling;

public class LotQuantitiesSentForDisposalIntegrationEventHandler(
    IDisposalOrderRepository disposalOrderRepo)
        : IIntegrationEventHandler<LotQuantitiesSentForDisposalIntegrationEvent>
{
    private readonly IDisposalOrderRepository _disposalOrderRepo = disposalOrderRepo;

    public async Task Handle(LotQuantitiesSentForDisposalIntegrationEvent @event)
    {
        var disposalQuantities = @event.DisposalLotQuantities
            .Select(x => new DisposalQuantity(x.LotNumber, x.DisposalQuantity, x.InventoryOwnerId));
        var disposalOrder = new DisposalOrder(disposalQuantities, @event.IsInitiatedByBazaar);

        _disposalOrderRepo.Create(disposalOrder);
        await Task.CompletedTask;
    }
}

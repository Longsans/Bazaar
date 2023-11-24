namespace Bazaar.Transport.ServiceIntegration.EventHandling;

public class LotUnitsLabeledForReturnIntegrationEventHandler
    : IIntegrationEventHandler<LotUnitsLabeledForReturnIntegrationEvent>
{
    private readonly IInventoryReturnRepository _returnRepo;
    private readonly IEstimationService _estimationService;

    public LotUnitsLabeledForReturnIntegrationEventHandler(
        IInventoryReturnRepository returnRepo, IEstimationService estimationService)
    {
        _returnRepo = returnRepo;
        _estimationService = estimationService;
    }

    public async Task Handle(LotUnitsLabeledForReturnIntegrationEvent @event)
    {
        var returnQuantities = @event.UnitsFromLots.Select(
            x => new ReturnQuantity(x.LotNumber, x.Units));
        var expectedDeliveryDate = _estimationService
            .EstimateInventoryReturnCompletion(returnQuantities);
        var inventoryReturn = new InventoryReturn(@event.DeliveryAddress, returnQuantities,
            expectedDeliveryDate, @event.InventoryOwnerId);

        _returnRepo.Create(inventoryReturn);
        await Task.CompletedTask;
    }
}

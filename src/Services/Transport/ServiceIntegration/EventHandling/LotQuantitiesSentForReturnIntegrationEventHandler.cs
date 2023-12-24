namespace Bazaar.Transport.ServiceIntegration.EventHandling;

public class LotQuantitiesSentForReturnIntegrationEventHandler
    : IIntegrationEventHandler<LotQuantitiesSentForReturnIntegrationEvent>
{
    private readonly IInventoryReturnRepository _returnRepo;
    private readonly IEstimationService _estimationService;

    public LotQuantitiesSentForReturnIntegrationEventHandler(
        IInventoryReturnRepository returnRepo, IEstimationService estimationService)
    {
        _returnRepo = returnRepo;
        _estimationService = estimationService;
    }

    public async Task Handle(LotQuantitiesSentForReturnIntegrationEvent @event)
    {
        var returnQuantities = @event.LotQuantities.Select(
            x => new ReturnQuantity(x.LotNumber, x.Quantity));
        var expectedDeliveryDate = _estimationService
            .EstimateInventoryReturnCompletion(returnQuantities);
        var inventoryReturn = new InventoryReturn(@event.DeliveryAddress, returnQuantities,
            expectedDeliveryDate, @event.InventoryOwnerId);

        _returnRepo.Create(inventoryReturn);
        await Task.CompletedTask;
    }
}

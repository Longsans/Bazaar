namespace Bazaar.Transport.ServiceIntegration.EventHandling;

public class LotQuantitiesSentForReturnIntegrationEventHandler
    : IIntegrationEventHandler<LotQuantitiesSentForReturnIntegrationEvent>
{
    private readonly IRepository<InventoryReturn> _returnRepo;
    private readonly IEstimationService _estimationService;

    public LotQuantitiesSentForReturnIntegrationEventHandler(
        IRepository<InventoryReturn> returnRepo, IEstimationService estimationService)
    {
        _returnRepo = returnRepo;
        _estimationService = estimationService;
    }

    public async Task Handle(LotQuantitiesSentForReturnIntegrationEvent @event)
    {
        var returnQuantities = @event.LotQuantities.Select(
            x => new ReturnQuantity(x.LotNumber, x.Quantity));
        var expectedDeliveryDate = await _estimationService
            .EstimateInventoryReturnCompletion(returnQuantities);
        var inventoryReturn = new InventoryReturn(@event.DeliveryAddress, returnQuantities,
            expectedDeliveryDate, @event.InventoryOwnerId);

        await _returnRepo.AddAsync(inventoryReturn);
        await Task.CompletedTask;
    }
}

namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class InventoryReturnCancelledIntegrationEventHandler
    : IIntegrationEventHandler<InventoryReturnCancelledIntegrationEvent>
{
    private readonly RemovalService _removalService;
    private readonly ILogger<InventoryReturnCancelledIntegrationEventHandler> _logger;

    public InventoryReturnCancelledIntegrationEventHandler(RemovalService removalService,
        ILogger<InventoryReturnCancelledIntegrationEventHandler> logger)
    {
        _removalService = removalService;
        _logger = logger;
    }

    public async Task Handle(InventoryReturnCancelledIntegrationEvent @event)
    {
        var restoreQuantities = @event.UnreturnedLotQuantities.ToDictionary(x => x.LotNumber, x => x.Quantity);
        var result = _removalService.RestoreLotUnitsFromRemoval(restoreQuantities);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error restoring units to lots for return order {ReturnId}: {ErrorMessage}",
                @event.ReturnId, result.GetJoinedErrorMessage());
        }
        await Task.CompletedTask;
    }
}

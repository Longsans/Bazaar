namespace Bazaar.FbbInventory.Application.EventHandling;

public class DisposalOrderCancelledIntegrationEventHandler
    : IIntegrationEventHandler<DisposalOrderCancelledIntegrationEvent>
{
    private readonly RemovalService _removalService;
    private readonly ILogger<DisposalOrderCancelledIntegrationEventHandler> _logger;

    public DisposalOrderCancelledIntegrationEventHandler(RemovalService removalService,
        ILogger<DisposalOrderCancelledIntegrationEventHandler> logger)
    {
        _removalService = removalService;
        _logger = logger;
    }

    public async Task Handle(DisposalOrderCancelledIntegrationEvent @event)
    {
        var restoreQuantities = @event.UndisposedQuantities.ToDictionary(x => x.LotNumber, x => x.UndisposedUnits);
        var result = await _removalService.RestoreLotUnitsFromRemoval(restoreQuantities);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error restoring units to lots for disposal order {DisposalOrderId}: {ErrorMessage}",
                @event.DisposalOrderId, result.GetJoinedErrorMessage());
        }

        await Task.CompletedTask;
    }
}

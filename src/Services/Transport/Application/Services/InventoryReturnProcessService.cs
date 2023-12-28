namespace Bazaar.Transport.Domain.Services;

public class InventoryReturnProcessService
{
    private readonly Repository<InventoryReturn> _returnRepo;
    private readonly IEventBus _eventBus;

    public InventoryReturnProcessService(
        Repository<InventoryReturn> returnRepository, IEventBus eventBus)
    {
        _returnRepo = returnRepository;
        _eventBus = eventBus;
    }

    public async Task<Result> StartReturnDelivery(int returnId)
    {
        return await UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.StartDelivery());
    }

    public async Task<Result> PostponeInventoryReturn(int returnId)
    {
        return await UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Postpone());
    }

    public async Task<Result> CompleteInventoryReturn(int returnId)
    {
        return await UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Complete(),
            PublishCompletedEvent);
    }

    public async Task<Result> CancelInventoryReturn(int returnId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Conflict("Cancel reason cannot be empty.");
        }
        return await UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Cancel(reason),
            PublishCancelledEvent);
    }

    // Helpers
    private async Task<Result> UpdateStatusAndPublishEvent(int returnId,
        Action<InventoryReturn> statusUpdate, Action<InventoryReturn>? publishEvent = null)
    {
        var invReturn = await _returnRepo
            .SingleOrDefaultAsync(new ReturnByIdSpec(returnId));
        if (invReturn == null)
        {
            return Result.NotFound();
        }

        try
        {
            statusUpdate(invReturn);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _returnRepo.UpdateAsync(invReturn);

        publishEvent?.Invoke(invReturn);
        return Result.Success();
    }

    private void PublishCompletedEvent(InventoryReturn invReturn)
    {
        var returnedLotUnits = invReturn.ReturnQuantities
            .Select(x => new LotQuantity(x.LotNumber, x.Quantity));
        _eventBus.Publish(new InventoryReturnCompletedIntegrationEvent(
            invReturn.Id, returnedLotUnits));
    }

    private void PublishCancelledEvent(InventoryReturn invReturn)
    {
        var returnedLotUnits = invReturn.ReturnQuantities
            .Select(x => new LotQuantity(x.LotNumber, x.Quantity));
        _eventBus.Publish(new InventoryReturnCancelledIntegrationEvent(
            invReturn.Id, returnedLotUnits));
    }
}

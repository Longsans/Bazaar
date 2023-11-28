namespace Bazaar.Transport.Domain.Services;

public class InventoryReturnProcessService
{
    private readonly IInventoryReturnRepository _returnRepo;
    private readonly IEventBus _eventBus;

    public InventoryReturnProcessService(
        IInventoryReturnRepository returnRepository, IEventBus eventBus)
    {
        _returnRepo = returnRepository;
        _eventBus = eventBus;
    }

    public Result StartReturnDelivery(int returnId)
    {
        return UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.StartDelivery());
    }

    public Result PostponeInventoryReturn(int returnId)
    {
        return UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Postpone());
    }

    public Result CompleteInventoryReturn(int returnId)
    {
        return UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Complete(),
            PublishCompletedEvent);
    }

    public Result CancelInventoryReturn(int returnId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Conflict("Cancel reason cannot be empty.");
        }
        return UpdateStatusAndPublishEvent(returnId,
            invReturn => invReturn.Cancel(reason),
            PublishCancelledEvent);
    }

    // Helpers
    private Result UpdateStatusAndPublishEvent(int returnId,
        Action<InventoryReturn> statusUpdate, Action<InventoryReturn>? publishEvent = null)
    {
        var invReturn = _returnRepo.GetById(returnId);
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
        _returnRepo.Update(invReturn);

        publishEvent?.Invoke(invReturn);
        return Result.Success();
    }

    private void PublishCompletedEvent(InventoryReturn invReturn)
    {
        var returnedLotUnits = invReturn.ReturnQuantities
            .Select(x => new UnitsFromLot(x.LotNumber, x.Units));
        _eventBus.Publish(new InventoryReturnCompletedIntegrationEvent(
            invReturn.Id, returnedLotUnits));
    }

    private void PublishCancelledEvent(InventoryReturn invReturn)
    {
        var returnedLotUnits = invReturn.ReturnQuantities
            .Select(x => new UnitsFromLot(x.LotNumber, x.Units));
        _eventBus.Publish(new InventoryReturnCancelledIntegrationEvent(
            invReturn.Id, returnedLotUnits));
    }
}

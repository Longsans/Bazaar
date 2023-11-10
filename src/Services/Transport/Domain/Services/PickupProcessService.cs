namespace Bazaar.Transport.Domain.Services;

public class PickupProcessService : IPickupProcessService
{
    private readonly IInventoryPickupRepository _pickupRepo;
    private readonly IEstimationService _estimationService;
    private readonly IEventBus _eventBus;

    public PickupProcessService(
        IInventoryPickupRepository pickupRepo, IEstimationService estimationService, IEventBus eventBus)
    {
        _pickupRepo = pickupRepo;
        _estimationService = estimationService;
        _eventBus = eventBus;
    }

    public Result<InventoryPickup> SchedulePickup(string pickupLocation,
        IEnumerable<ProductInventory> pickupInventories, string schedulerId)
    {
        var estimatedPickupTime = _estimationService
            .EstimatePickupCompletion(pickupInventories);

        try
        {
            var pickup = new InventoryPickup(
                pickupLocation,
                pickupInventories,
                estimatedPickupTime,
                schedulerId);

            _pickupRepo.Create(pickup);
            PublishProductInventoryPickupStatuses(pickup);
            return pickup;
        }
        catch (Exception ex)
        {
            return Result.Invalid(new ValidationError()
            {
                ErrorMessage = ex.Message
            });
        }
    }

    public Result DispatchPickup(int pickupId)
    {
        return ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.Start();
        }, true);
    }

    public Result ConfirmPickupInventory(int pickupId)
    {
        return ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.ConfirmInventoryPickedUp();
        }, false);
    }

    public Result CompletePickup(int pickupId)
    {
        return ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.Complete();
        }, true, true);
    }

    public Result CancelPickup(int pickupId, string reason)
    {
        return ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.Cancel(reason);
        }, true);
    }

    private void PublishProductInventoryPickupStatuses(InventoryPickup pickup)
    {
        foreach (var productInventory in pickup.ProductInventories)
        {
            var productId = productInventory.ProductId;
            var scheduledCount = (uint)_pickupRepo.GetScheduled(productId).Count();
            var inProgressCount = (uint)_pickupRepo.GetInProgress(productId).Count();
            var completedCount = (uint)_pickupRepo.GetCompleted(productId).Count();
            var cancelledCount = (uint)_pickupRepo.GetCancelled(productId).Count();
            var @event = new ProductInventoryPickupsStatusChangedIntegrationEvent(
                productId, scheduledCount, inProgressCount, completedCount, cancelledCount);

            _eventBus.Publish(@event);
        }
    }

    private Result ProceedWithPickup(
        int pickupId, Action<InventoryPickup> progression,
        bool publishStatusEvent = false,
        bool publishCompletedEvent = false)
    {
        var pickup = _pickupRepo.GetById(pickupId);
        if (pickup == null)
        {
            return Result.NotFound("Pickup not found.");
        }

        try
        {
            progression(pickup);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _pickupRepo.Update(pickup);
        if (publishStatusEvent)
        {
            PublishProductInventoryPickupStatuses(pickup);
        }
        if (publishCompletedEvent)
        {
            var @event = new InventoryPickedUpIntegrationEvent(
                pickup.ProductInventories.Select(
                    x => new PickupProductInventory(x.ProductId, x.NumberOfUnits)),
                pickup.SchedulerId);
            _eventBus.Publish(@event);
        }

        return Result.Success();
    }
}

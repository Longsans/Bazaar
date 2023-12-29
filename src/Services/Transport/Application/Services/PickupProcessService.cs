namespace Bazaar.Transport.Domain.Services;

public class PickupProcessService
{
    private readonly IRepository<InventoryPickup> _pickupRepo;
    private readonly IEstimationService _estimationService;
    private readonly IEventBus _eventBus;

    public PickupProcessService(
        IRepository<InventoryPickup> pickupRepo, IEstimationService estimationService, IEventBus eventBus)
    {
        _pickupRepo = pickupRepo;
        _estimationService = estimationService;
        _eventBus = eventBus;
    }

    public async Task<Result<InventoryPickup>> SchedulePickup(string pickupLocation,
        IEnumerable<PickupProductStock> pickupInventories, string schedulerId)
    {
        var estimatedPickupTime = await _estimationService
            .EstimatePickupCompletion(pickupInventories);

        try
        {
            var pickup = new InventoryPickup(
                pickupLocation,
                pickupInventories,
                estimatedPickupTime,
                schedulerId);

            await _pickupRepo.AddAsync(pickup);
            await PublishProductInventoryPickupStatuses(pickup);
            return pickup;
        }
        catch (Exception ex)
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(pickupInventories),
                ErrorMessage = ex.Message
            });
        }
    }

    public async Task<Result> DispatchPickup(int pickupId)
    {
        return await ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.StartPickup();
        }, true);
    }

    public async Task<Result> ConfirmPickupInventory(int pickupId)
    {
        return await ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.ConfirmInventoryPickedUp();
        }, false);
    }

    public async Task<Result> CompletePickup(int pickupId)
    {
        return await ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.Complete();
        }, true, true);
    }

    public async Task<Result> CancelPickup(int pickupId, string reason)
    {
        return await ProceedWithPickup(pickupId, (pickup) =>
        {
            pickup.Cancel(reason);
        }, true);
    }

    private async Task PublishProductInventoryPickupStatuses(InventoryPickup pickup)
    {
        foreach (var productInventory in pickup.ProductStocks)
        {
            var productId = productInventory.ProductId;
            var pickups = await _pickupRepo.ListAsync(
                new PickupsContainingProductSpec(productId));

            var scheduledCount = new PickupsScheduledSpec().Evaluate(pickups).Count();
            var inProgressCount = new PickupsInProgressSpec().Evaluate(pickups).Count();
            var completedCount = new PickupsCompletedSpec().Evaluate(pickups).Count();
            var cancelledCount = new PickupsCancelledSpec().Evaluate(pickups).Count();

            var @event = new ProductFbbInventoryPickupsStatusChangedIntegrationEvent(
                productId, (uint)scheduledCount, (uint)inProgressCount,
                (uint)completedCount, (uint)cancelledCount);
            _eventBus.Publish(@event);
        }
    }

    #region Helpers
    private async Task<Result> ProceedWithPickup(
        int pickupId, Action<InventoryPickup> progression,
        bool publishStatusEvent = false,
        bool publishCompletedEvent = false)
    {
        var pickup = await _pickupRepo.SingleOrDefaultAsync(new PickupByIdSpec(pickupId));
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

        await _pickupRepo.UpdateAsync(pickup);
        if (publishStatusEvent)
        {
            await PublishProductInventoryPickupStatuses(pickup);
        }
        if (publishCompletedEvent)
        {
            var @event = new FbbInventoryPickedUpIntegrationEvent(
                pickup.ProductStocks.Select(
                    x => new PickupProductInventory(x.ProductId, x.NumberOfUnits)),
                pickup.SchedulerId);
            _eventBus.Publish(@event);
        }

        return Result.Success();
    }
    #endregion
}

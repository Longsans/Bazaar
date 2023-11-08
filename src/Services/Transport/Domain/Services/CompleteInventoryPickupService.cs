namespace Bazaar.Transport.Domain.Services;

public class CompleteInventoryPickupService : ICompleteInventoryPickupService
{
    private readonly IEventBus _eventBus;
    private readonly IInventoryPickupRepository _pickupRepo;

    public CompleteInventoryPickupService(
        IEventBus eventBus, IInventoryPickupRepository pickupRepo)
    {
        _eventBus = eventBus;
        _pickupRepo = pickupRepo;
    }

    public Result CompleteInventoryPickup(int id)
    {
        var pickup = _pickupRepo.GetById(id);
        if (pickup == null)
        {
            return Result.NotFound("Inventory pickup not found.");
        }

        try
        {
            pickup.Complete();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _pickupRepo.Update(pickup);
        var @event = new InventoryPickedUpIntegrationEvent(
            pickup.ProductInventories.Select(x => new PickupProductInventory(x.ProductId, x.NumberOfUnits)),
            pickup.SchedulerId);
        _eventBus.Publish(@event);
        return Result.Success();
    }
}

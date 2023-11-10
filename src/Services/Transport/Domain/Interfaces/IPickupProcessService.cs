namespace Bazaar.Transport.Domain.Interfaces;

public interface IPickupProcessService
{
    Result<InventoryPickup> SchedulePickup(string pickupLocation,
        IEnumerable<ProductInventory> pickupInventories, string schedulerId);
    Result DispatchPickup(int pickupId);
    Result ConfirmPickupInventory(int pickupId);
    Result CompletePickup(int pickupId);
    Result CancelPickup(int pickupId, string reason);
}

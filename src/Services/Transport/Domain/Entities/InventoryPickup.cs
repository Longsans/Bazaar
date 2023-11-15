namespace Bazaar.Transport.Domain.Entities;

public class InventoryPickup
{
    public int Id { get; private set; }
    public string PickupLocation { get; private set; }
    public List<ProductInventory> ProductInventories { get; private set; }
    public DateTime EstimatedPickupTime { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string SchedulerId { get; private set; }
    public InventoryPickupStatus Status { get; private set; }

    public string? CancelReason { get; private set; }

    public InventoryPickup(
        string pickupLocation, IEnumerable<ProductInventory> productInventories,
        DateTime estimatedPickupTime, string schedulerId)
    {
        if (string.IsNullOrWhiteSpace(pickupLocation))
        {
            throw new ArgumentNullException(
                nameof(pickupLocation), "Pickup location cannot be empty.");
        }

        if (estimatedPickupTime < DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimatedPickupTime),
                "Estimated pickup time must be after current time.");
        }

        if (!productInventories.Any())
        {
            throw new ArgumentNullException(nameof(productInventories),
                "Inventory items cannot be empty.");
        }

        bool hasRepeatedProducts = productInventories
            .GroupBy(x => x.ProductId)
            .Select(g => g.Count())
            .Any(c => c > 1);
        if (hasRepeatedProducts)
        {
            throw new ArgumentException(
                "Product inventories cannot be duplicate.", nameof(productInventories));
        }

        PickupLocation = pickupLocation;
        ProductInventories = productInventories.ToList();
        EstimatedPickupTime = estimatedPickupTime;
        ScheduledAt = DateTime.Now;
        SchedulerId = schedulerId;
        Status = InventoryPickupStatus.Scheduled;
    }

    // EF read constructor
    private InventoryPickup() { }

    public void StartPickup()
    {
        Status = Status == InventoryPickupStatus.Scheduled
            ? InventoryPickupStatus.EnRouteToPickupLocation
            : throw new InvalidOperationException(
                "Can only start pickup if it is currently in \"Scheduled\" status.");
    }

    public void ConfirmInventoryPickedUp()
    {
        Status = Status == InventoryPickupStatus.EnRouteToPickupLocation
            ? InventoryPickupStatus.DeliveringToWarehouse
            : throw new InvalidOperationException(
                "Can only confirm inventory picked up if " +
                "pickup is currently in \"EnRouteToPickupLocation\" status.");
    }

    public void Complete()
    {
        Status = Status == InventoryPickupStatus.DeliveringToWarehouse
            ? InventoryPickupStatus.Completed
            : throw new InvalidOperationException(
                "Can only complete pickup if its currently in \"DeliveringToWarehouse\" status.");
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentNullException(
                nameof(reason), "Cancel reason cannot be empty.");
        }

        Status = Status != InventoryPickupStatus.Completed && Status != InventoryPickupStatus.Cancelled
            ? InventoryPickupStatus.Cancelled
            : throw new InvalidOperationException(
                "Cannot cancel a completed or an already cancelled inventory pickup.");
        CancelReason = reason;
    }
}

public enum InventoryPickupStatus
{
    Scheduled,
    EnRouteToPickupLocation,
    DeliveringToWarehouse,
    Completed,
    Cancelled
}

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
        string pickupLocation, IEnumerable<ProductInventory> inventoryItems,
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

        if (!inventoryItems.Any())
        {
            throw new ArgumentException(
                "Inventory items cannot be empty.", nameof(inventoryItems));
        }

        if (inventoryItems.Any(x => x.NumberOfUnits == 0))
        {
            throw new ArgumentOutOfRangeException(
                nameof(inventoryItems), "Number of units in product inventory cannot be 0.");
        }

        bool hasRepeatedProducts = inventoryItems
            .GroupBy(x => x.ProductId)
            .Select(g => g.Count())
            .Any(c => c > 1);
        if (hasRepeatedProducts)
        {
            throw new ArgumentException(
                "Product inventories cannot be duplicate.", nameof(inventoryItems));
        }

        PickupLocation = pickupLocation;
        ProductInventories = inventoryItems.ToList();
        EstimatedPickupTime = estimatedPickupTime;
        ScheduledAt = DateTime.Now;
        SchedulerId = schedulerId;
        Status = InventoryPickupStatus.Scheduled;
    }

    // EF read constructor
    private InventoryPickup() { }

    public void UpdateEstimatedPickupTime(DateTime estimatedTime)
    {
        if (estimatedTime < DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimatedTime), "Estimated pickup time must be after current time.");
        }

        EstimatedPickupTime = estimatedTime;
    }

    public void Start()
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
                "Cannot confirm that inventory is picked up before pickup is en route.");
    }

    public void Complete()
    {
        Status = Status == InventoryPickupStatus.DeliveringToWarehouse
            ? InventoryPickupStatus.Completed
            : throw new InvalidOperationException(
                "Cannot complete pickup until it is confirmed that inventory has been picked up.");
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentNullException(
                nameof(reason), "Cancel reason cannot be empty.");
        }

        Status = Status != InventoryPickupStatus.Completed
            ? InventoryPickupStatus.Cancelled
            : throw new InvalidOperationException(
                "Cannot cancel a completed inventory pickup.");
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

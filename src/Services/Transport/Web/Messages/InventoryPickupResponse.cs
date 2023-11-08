namespace Bazaar.Transport.Web.Messages;

public class InventoryPickupResponse
{
    public int Id { get; private set; }
    public string PickupLocation { get; private set; }
    public List<ProductInventoryResponse> ProductInventories { get; private set; }
    public DateTime EstimatedPickupTime { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public string SchedulerId { get; private set; }
    public InventoryPickupStatus Status { get; private set; }

    public bool IsCancelled { get; private set; }

    public InventoryPickupResponse(InventoryPickup pickup)
    {
        Id = pickup.Id;
        PickupLocation = pickup.PickupLocation;
        ProductInventories = pickup.ProductInventories
            .Select(x => new ProductInventoryResponse(x)).ToList();
        EstimatedPickupTime = pickup.EstimatedPickupTime;
        ScheduledAt = pickup.ScheduledAt;
        SchedulerId = pickup.SchedulerId;
        Status = pickup.Status;
        IsCancelled = pickup.IsCancelled;
    }
}

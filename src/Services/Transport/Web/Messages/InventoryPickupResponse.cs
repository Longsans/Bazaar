namespace Bazaar.Transport.Web.Messages;

public class InventoryPickupResponse
{
    public int Id { get; private set; }
    public string PickupLocation { get; private set; }
    public List<ProductInventoryResponse> ProductInventories { get; private set; }
    public DateTime TimeScheduledAt { get; private set; }
    public DateTime EstimatedPickupTime { get; private set; }
    public string SchedulerId { get; private set; }
    public string Status { get; private set; }

    public InventoryPickupResponse(InventoryPickup pickup)
    {
        Id = pickup.Id;
        PickupLocation = pickup.PickupLocation;
        ProductInventories = pickup.ProductStocks
            .Select(x => new ProductInventoryResponse(x)).ToList();
        TimeScheduledAt = pickup.TimeScheduledAt;
        EstimatedPickupTime = pickup.EstimatedPickupTime;
        SchedulerId = pickup.SchedulerId;
        Status = Enum.GetName(pickup.Status)!;
    }
}

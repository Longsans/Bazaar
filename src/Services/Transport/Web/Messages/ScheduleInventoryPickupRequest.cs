namespace Bazaar.Transport.Web.Messages;

public class ScheduleInventoryPickupRequest
{
    public string PickupLocation { get; set; }
    public List<InventoryItemRequest> InventoryItems { get; set; }
    public string SchedulerId { get; set; }
}

public class InventoryItemRequest
{
    public string ProductId { get; set; }
    public uint NumberOfUnits { get; set; }
}
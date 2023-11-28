namespace Bazaar.Transport.Web.Messages;

public record struct ScheduleDeliveryRequest(
    int OrderId, string DeliveryAddress,
    IEnumerable<DeliveryRequestPackageItem> PackageItems);

public class DeliveryRequestPackageItem
{
    public string ProductId { get; set; }
    public uint Quantity { get; set; }
}
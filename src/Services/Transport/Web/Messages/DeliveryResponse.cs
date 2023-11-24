namespace Bazaar.Transport.Web.Messages;

public class DeliveryResponse
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string DeliveryAddress { get; private set; }
    public List<PackageItemResponse> Items { get; private set; }
    public DateTime TimeScheduledAt { get; private set; }
    public DateTime EstimatedDeliveryTime { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public DeliveryResponse(Delivery delivery)
    {
        Id = delivery.Id;
        OrderId = delivery.OrderId;
        DeliveryAddress = delivery.DeliveryAddress;
        Items = delivery.PackageItems
            .Select(x => new PackageItemResponse(x)).ToList();
        TimeScheduledAt = delivery.TimeScheduledAt;
        EstimatedDeliveryTime = delivery.EstimatedDeliveryTime;
        Status = delivery.Status;
    }
}

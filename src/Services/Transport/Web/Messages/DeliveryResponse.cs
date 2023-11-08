namespace Bazaar.Transport.Web.Messages;

public class DeliveryResponse
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public List<PackageItemResponse> Items { get; private set; }
    public DateTime ScheduledAtDate { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public DeliveryResponse(Delivery delivery)
    {
        Id = delivery.Id;
        OrderId = delivery.OrderId;
        Items = delivery.Items
            .Select(x => new PackageItemResponse(x)).ToList();
        ScheduledAtDate = delivery.ScheduledAtDate;
        ExpectedDeliveryDate = delivery.ExpectedDeliveryDate;
        Status = delivery.Status;
    }
}

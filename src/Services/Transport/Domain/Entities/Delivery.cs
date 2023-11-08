namespace Bazaar.Transport.Domain.Entities;

public class Delivery
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public List<DeliveryPackageItem> Items { get; private set; }
    public DateTime ScheduledAtDate { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public Delivery(
        int orderId, IEnumerable<DeliveryPackageItem> items,
        DateTime expectedDeliveryDate)
    {
        OrderId = orderId;
        Items = items.ToList();
        ScheduledAtDate = DateTime.Now.Date;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Status = DeliveryStatus.Scheduled;
    }

    // EF read constructor
    private Delivery() { }

    public void StartDelivery()
    {
        Status = Status == DeliveryStatus.Scheduled || Status == DeliveryStatus.Postponed
            ? DeliveryStatus.Delivering
            : throw new InvalidOperationException(
                "Can only start delivery if it is currently in \"Scheduled\" or \"Postponed\" status.");
    }

    public void Complete()
    {
        Status = Status == DeliveryStatus.Delivering
            ? DeliveryStatus.Completed
            : throw new InvalidOperationException(
                "Cannot complete delivery before it starts.");
    }

    public void Postpone()
    {
        Status = Status != DeliveryStatus.Completed
            ? DeliveryStatus.Postponed
            : throw new InvalidOperationException(
                "Cannot postpone a completed delivery.");
    }
}

public enum DeliveryStatus
{
    Scheduled,
    Delivering,
    Completed,
    Postponed
}
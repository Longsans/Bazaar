namespace Bazaar.Transport.Domain.Entities;

public class Delivery
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string DeliveryAddress { get; private set; }
    public List<DeliveryPackageItem> PackageItems { get; private set; }
    public DateTime ScheduledAtDate { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public Delivery(
        int orderId, string deliveryAddress,
        IEnumerable<DeliveryPackageItem> packageItems,
        DateTime expectedDeliveryDate)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            throw new ArgumentNullException(nameof(deliveryAddress),
                "Delivery address cannot be empty.");
        }
        if (!packageItems.Any())
        {
            throw new ArgumentNullException(nameof(packageItems),
                "Package items cannot be empty.");
        }
        if (expectedDeliveryDate < DateTime.Now.Date)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedDeliveryDate),
                "Expected delivery date cannot be before current date.");
        }

        var hasDuplicateProducts = packageItems
            .GroupBy(x => x.ProductId)
            .Select(g => g.Count())
            .Any(c => c > 1);
        if (hasDuplicateProducts)
        {
            throw new ArgumentException(
                "Package items cannot be duplicate.", nameof(packageItems));
        }

        OrderId = orderId;
        DeliveryAddress = deliveryAddress;
        PackageItems = packageItems.ToList();
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

    public void Cancel()
    {
        Status = Status != DeliveryStatus.Completed
            ? DeliveryStatus.Cancelled
            : throw new InvalidOperationException(
                "Cannot cancel a completed delivery.");
    }
}

public enum DeliveryStatus
{
    Scheduled,
    Delivering,
    Completed,
    Postponed,
    Cancelled
}
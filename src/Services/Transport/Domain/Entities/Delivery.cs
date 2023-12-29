namespace Bazaar.Transport.Domain.Entities;

public class Delivery
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string DeliveryAddress { get; private set; }

    public List<DeliveryPackageItem> _packageItems;
    public IReadOnlyCollection<DeliveryPackageItem> PackageItems
        => _packageItems.AsReadOnly();
    public DateTime TimeScheduledAt { get; private set; }
    public DateTime EstimatedDeliveryTime { get; private set; }
    public DeliveryStatus Status { get; private set; }

    public Delivery(
        int orderId, string deliveryAddress,
        IEnumerable<DeliveryPackageItem> packageItems,
        DateTime estimateDeliveryTime)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            throw new ArgumentException(nameof(deliveryAddress),
                "Delivery address cannot be empty.");
        }
        if (!packageItems.Any())
        {
            throw new ArgumentException(nameof(packageItems),
                "Package items cannot be empty.");
        }
        if (estimateDeliveryTime < DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimateDeliveryTime),
                "Expected delivery date cannot be before current date.");
        }

        var hasDuplicateProducts = packageItems
            .GroupBy(x => x.ProductId)
            .Any(g => g.Count() > 1);
        if (hasDuplicateProducts)
        {
            throw new ArgumentException(
                "Package items cannot be duplicate.", nameof(packageItems));
        }

        OrderId = orderId;
        DeliveryAddress = deliveryAddress;
        _packageItems = packageItems.ToList();
        TimeScheduledAt = DateTime.Now;
        EstimatedDeliveryTime = estimateDeliveryTime;
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
        Status = Status != DeliveryStatus.Completed && Status != DeliveryStatus.Cancelled
            ? DeliveryStatus.Postponed
            : throw new InvalidOperationException(
                "Cannot postpone a completed or cancelled delivery.");
    }

    public void Cancel()
    {
        Status = Status != DeliveryStatus.Completed && Status != DeliveryStatus.Cancelled
            ? DeliveryStatus.Cancelled
            : throw new InvalidOperationException(
                "Cannot cancel a completed or an already cancelled delivery.");
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
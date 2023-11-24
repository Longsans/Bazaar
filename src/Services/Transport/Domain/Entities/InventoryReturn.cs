namespace Bazaar.Transport.Domain.Entities;

public class InventoryReturn
{
    public int Id { get; private set; }
    public string DeliveryAddress { get; private set; }

    private readonly List<ReturnQuantity> _returnQuantities;
    public IReadOnlyCollection<ReturnQuantity> ReturnQuantities
        => _returnQuantities.AsReadOnly();
    public DateTime TimeScheduledAt { get; private set; }
    public DateTime EstimatedDeliveryTime { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public string? CancelReason { get; private set; }

    public string InventoryOwnerId { get; private set; }

    public InventoryReturn(string deliveryAddress,
        IEnumerable<ReturnQuantity> returnQuantities,
        DateTime estimatedDeliveryTime, string inventoryOwnerId)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            throw new ArgumentNullException(nameof(deliveryAddress),
                "Delivery address cannot be empty.");
        }
        if (estimatedDeliveryTime < DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(nameof(estimatedDeliveryTime),
                "Expected delivery date cannot be before current date.");
        }
        if (string.IsNullOrWhiteSpace(inventoryOwnerId))
        {
            throw new ArgumentNullException(nameof(inventoryOwnerId),
                "Inventory owner ID cannot be empty.");
        }

        var hasDuplicateLots = returnQuantities.GroupBy(x => x.LotNumber)
            .Any(g => g.Count() > 1);
        if (hasDuplicateLots)
        {
            throw new ArgumentException("Return quantities cannot contain quantities from duplicate lots.",
                nameof(returnQuantities));
        }

        DeliveryAddress = deliveryAddress;
        _returnQuantities = returnQuantities.ToList();
        TimeScheduledAt = DateTime.Now;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        Status = DeliveryStatus.Scheduled;
        InventoryOwnerId = inventoryOwnerId;
    }

    private InventoryReturn() { }

    public void StartDelivery()
    {
        Status = Status == DeliveryStatus.Scheduled || Status == DeliveryStatus.Postponed
            ? DeliveryStatus.Delivering
            : throw new InvalidOperationException(
                "Can only start delivery if inventory return is currently in Scheduled or Postponed status.");
    }

    public void Complete()
    {
        Status = Status == DeliveryStatus.Delivering
            ? DeliveryStatus.Completed
            : throw new InvalidOperationException(
                "Can only complete an inventory return if it's currently in Delivering status.");
    }

    public void Postpone()
    {
        Status = Status != DeliveryStatus.Completed && Status != DeliveryStatus.Cancelled
            ? DeliveryStatus.Postponed
            : throw new InvalidOperationException(
                "Cannot postpone a completed or cancelled inventory return.");
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentNullException(nameof(reason),
                "Reason cannot be empty.");
        }

        Status = Status != DeliveryStatus.Completed && Status != DeliveryStatus.Cancelled
            ? DeliveryStatus.Cancelled
            : throw new InvalidOperationException(
                "Cannot cancel a completed or an already cancelled inventory return.");
        CancelReason = reason;
    }
}

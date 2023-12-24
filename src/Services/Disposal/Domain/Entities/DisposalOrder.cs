namespace Bazaar.Disposal.Domain.Entities;

public class DisposalOrder
{
    public int Id { get; private set; }
    private readonly List<DisposalQuantity> _disposeQuantities;
    public IReadOnlyCollection<DisposalQuantity> DisposeQuantities
        => _disposeQuantities.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public bool CreatedByBazaar { get; private set; }
    public DisposalStatus Status { get; private set; }
    public string? CancelReason { get; private set; }

    public DisposalOrder(IEnumerable<DisposalQuantity> disposeQuantities, bool createdByBazaar)
    {
        if (!disposeQuantities.Any())
        {
            throw new ArgumentNullException(nameof(disposeQuantities),
                "List of quantities to dispose cannot be empty.");
        }

        var hasDuplicateQuantities = disposeQuantities
            .GroupBy(x => x.LotNumber)
            .Select(g => g.Count())
            .Any(x => x > 1);
        if (hasDuplicateQuantities)
        {
            throw new ArgumentException("Quantities cannot be duplicate.",
                nameof(disposeQuantities));
        }

        if (disposeQuantities.Any(x => x.UnitsToDispose == 0))
        {
            throw new ArgumentOutOfRangeException(nameof(disposeQuantities),
                "A quantity's units to dispose cannot be 0.");
        }

        _disposeQuantities = disposeQuantities.ToList();
        CreatedAt = DateTime.Now;
        Status = DisposalStatus.Pending;
        CreatedByBazaar = createdByBazaar;
    }

    // Yet another EF constructor
    private DisposalOrder() { }

    public void StartProcessing()
    {
        Status = Status == DisposalStatus.Pending
            ? DisposalStatus.Processing
            : throw new InvalidOperationException(
                "Can only start processing disposal if it's currently in Pending status.");
    }

    public void Complete()
    {
        Status = Status == DisposalStatus.Processing
            ? DisposalStatus.Completed
            : throw new InvalidOperationException(
                "Can only complete disposal if it's currently in Processing status.");
    }

    public void Cancel(string reason)
    {
        if (CreatedByBazaar)
        {
            throw new InvalidOperationException(
                "Disposal orders created by Bazaar cannot be cancelled.");
        }
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentNullException(nameof(reason),
                "Cancel reason cannot be empty.");
        }

        Status = Status == DisposalStatus.Pending
            ? DisposalStatus.Cancelled
            : throw new InvalidOperationException(
                "Can only cancel disposal if it's currently in Pending status.");

        CancelReason = reason;
    }
}

public enum DisposalStatus
{
    Pending,
    Processing,
    Completed,
    Cancelled
}
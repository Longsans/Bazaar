namespace Bazaar.FbbInventory.Domain.ValueObjects;

/// <summary>
/// An issue of stock. Can contain items from lots of multiple products.
/// The <see cref="IsFinalized"/> property should <strong>always</strong> be checked
/// before attempting to serialize--if value is not true, call <see cref="FinalizeIssue"/> first.
/// </summary>
public record StockIssue
{
    public DateTime DateOfIssue { get; }
    public StockIssueReason IssueReason { get; }

    private readonly List<StockIssueItem> _items;
    public IReadOnlyCollection<StockIssueItem> Items => _items.AsReadOnly();

    public StockIssue(IEnumerable<StockIssueItem> issueItems, StockIssueReason issueReason)
    {
        if (issueItems.GroupBy(x => x.LotNumber).Any(g => g.Count() > 1))
        {
            throw new ArgumentException("Each lot can only appear once in issue items list.");
        }

        DateOfIssue = DateTime.Now.Date;
        IssueReason = issueReason;
        _items = issueItems.ToList();
    }

    public StockIssue(StockIssueReason issueReason) : this(new List<StockIssueItem>(), issueReason)
    {
    }

    private StockIssue() { }

    public StockIssue AddItems(params StockIssueItem[] items)
    {
        return AddItems(items.AsEnumerable());
    }

    public StockIssue RemoveItems(params StockIssueItem[] items)
    {
        return RemoveItems(items.AsEnumerable());
    }

    public StockIssue AddItems(IEnumerable<StockIssueItem> items)
    {
        if (items.Any(x => _items.Any(i => i.LotNumber == x.LotNumber)))
        {
            throw new ArgumentException("An item is duplicate of an existing item in issue.");
        }

        var issueItems = _items.ToList();
        issueItems.AddRange(items);
        return new StockIssue(issueItems, IssueReason);
    }

    public StockIssue RemoveItems(IEnumerable<StockIssueItem> items)
    {
        if (items.Any(x => !_items.Any(i => i.LotNumber == x.LotNumber)))
        {
            throw new ArgumentException("An item does not exist in issue items list.");
        }

        var issueItems = _items.Where(x => !items.Contains(x)).ToList();
        return new StockIssue(issueItems, IssueReason);
    }
}

public record StockIssueItem
{
    public string ProductId { get; }
    public string LotNumber { get; }
    public uint Quantity { get; }

    public StockIssueItem(string productId, string lotNumber, uint quantity)
    {
        if (quantity == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity),
                "Issue quantity cannot be 0.");
        }

        ProductId = productId;
        LotNumber = lotNumber;
        Quantity = quantity;
    }
}

public enum StockIssueReason
{
    Sale,
    Disposal,
    Return
}
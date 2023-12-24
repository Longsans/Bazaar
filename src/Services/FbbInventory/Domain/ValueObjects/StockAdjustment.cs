namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockAdjustment
{
    public int Id { get; private set; }
    public DateTime DateOfAdjustment { get; private set; }
    public bool IsFinalized { get; private set; }

    private readonly List<StockAdjustmentItem> _items;
    public IReadOnlyCollection<StockAdjustmentItem> Items => _items.AsReadOnly();

    public StockAdjustment(IEnumerable<StockAdjustmentItem> items, bool finalized = false)
    {
        if (!items.Any())
        {
            throw new ArgumentException("Adjustment items cannot be empty.");
        }
        if (items.GroupBy(x => x.LotNumber).Any(g => g.Count() > 1))
        {
            throw new ArgumentException("Each lot can only appear once in a stock adjustment.");
        }

        DateOfAdjustment = DateTime.Now.Date;
        IsFinalized = finalized;
        _items = items.ToList();
    }

    private StockAdjustment() { }

    public void AddItems(params StockAdjustmentItem[] items)
    {
        AddItems(items.AsEnumerable());
    }

    public void RemoveItems(params StockAdjustmentItem[] items)
    {
        RemoveItems(items.AsEnumerable());
    }

    public void AddItems(IEnumerable<StockAdjustmentItem> items)
    {
        if (IsFinalized)
        {
            throw new InvalidOperationException("Adjustment is finalized.");
        }
        if (items.Any(x => _items.Any(i => i.LotNumber == x.LotNumber)))
        {
            throw new ArgumentException("An item has duplicate lot number with an existing item.");
        }
        _items.AddRange(items);
    }

    public void RemoveItems(IEnumerable<StockAdjustmentItem> items)
    {
        if (IsFinalized)
        {
            throw new InvalidOperationException("Adjustment is finalized.");
        }
        if (items.Any(x => !_items.Any(i => i.LotNumber == x.LotNumber)))
        {
            throw new ArgumentException("An item does not exist in adjustment items list.");
        }

        foreach (var item in items)
        {
            _items.Remove(item);
        }
    }

    public void FinalizeAdjustment()
    {
        IsFinalized = true;
    }
}

public class StockAdjustmentItem
{
    public int Id { set; private get; }
    public string ProductId { get; private set; }
    public string LotNumber { get; private set; }
    public int QuantityAdjusted { get; private set; }
    public StockAdjustmentReason AdjustmentReason { get; private set; }

    public StockAdjustmentItem(string productId, string lotNumber,
        int quantityAdjusted, StockAdjustmentReason adjustmentReason)
    {
        if (quantityAdjusted == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityAdjusted),
                "Quantity adjusted cannot be 0.");
        }

        switch (adjustmentReason)
        {
            case StockAdjustmentReason.RemoveUnfulfillableGoods when quantityAdjusted > 0:
            case StockAdjustmentReason.Theft when quantityAdjusted > 0:
                throw new ArgumentException("Adjustment reason cannot be Unfulfillable Goods or Theft " +
                    "when quantity after adjustment is not smaller than quantity before adjustment.");
            case StockAdjustmentReason.CountDiscrepancy:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(adjustmentReason),
                    "Invalid adjustment reason.");
        }

        ProductId = productId;
        LotNumber = lotNumber;
        QuantityAdjusted = quantityAdjusted;
        AdjustmentReason = adjustmentReason;
    }

    public StockAdjustmentItem(string productId, string lotNumber,
        uint quantityAdjusted, StockAdjustmentReason adjustmentReason)
        : this(productId, lotNumber, (int)quantityAdjusted, adjustmentReason)
    {

    }

    private StockAdjustmentItem() { }
}

public enum StockAdjustmentReason
{
    RemoveUnfulfillableGoods,
    CountDiscrepancy,
    Theft,
    Reclassification
}
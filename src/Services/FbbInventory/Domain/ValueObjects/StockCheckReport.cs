namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockCheckReport
{
    private readonly List<StockCheckReportItem> _items;
    public IReadOnlyCollection<StockCheckReportItem> Items => _items.AsReadOnly();
    public bool CanSatisfyDemand => _items.All(x => x.CanSatisfyDemand);

    public StockCheckReport(IEnumerable<StockCheckReportItem> items)
    {
        _items = items.ToList();
    }
}

public class StockCheckReportItem
{
    public string ProductId { get; private set; }
    public uint DemandedGoodUnits { get; private set; }
    public uint DemandedUnfulfillableUnits { get; private set; }
    public uint GoodStockCount { get; private set; }
    public uint UnfulfillableStockCount { get; private set; }
    public bool IsListed { get; private set; }
    public bool CanSatisfyDemand => GoodStockCount >= DemandedGoodUnits && UnfulfillableStockCount >= DemandedUnfulfillableUnits && IsListed;

    public StockCheckReportItem(string productId, uint demandedGoodUnits, uint demandedUnfulfillableUnits, uint goodStockCount, uint unfulfillableStockCount, bool listed = true)
    {
        ProductId = productId;
        DemandedGoodUnits = demandedGoodUnits;
        DemandedUnfulfillableUnits = demandedUnfulfillableUnits;
        GoodStockCount = goodStockCount;
        UnfulfillableStockCount = unfulfillableStockCount;
        IsListed = listed;
    }

    public static StockCheckReportItem UnlistedItem(string productId) => new(
        productId, 0u, 0u, 0u, 0u, false);
}

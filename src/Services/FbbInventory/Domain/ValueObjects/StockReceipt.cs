namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockReceipt
{
    public DateTime DateOfReceipt { get; private set; }
    private readonly List<StockReceiptItem> _items;
    public IReadOnlyCollection<StockReceiptItem> Items => _items.AsReadOnly();

    public StockReceipt(IEnumerable<StockReceiptItem> items)
    {
        DateOfReceipt = DateTime.Now.Date;
        _items = items.ToList();
    }
}

public class StockReceiptItem
{
    public string ProductId { get; private set; }
    public uint GoodQuantity { get; private set; }
    public uint DefectiveQuantity { get; private set; }
    public uint WarehouseDamagedQuantity { get; private set; }

    public StockReceiptItem(string productId, uint fulfillableQuantity,
        uint defectiveQuantity, uint warehouseDamagedQuantity)
    {
        ProductId = productId;
        GoodQuantity = fulfillableQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
    }
}
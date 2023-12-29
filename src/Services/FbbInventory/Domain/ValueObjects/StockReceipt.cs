namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockReceipt
{
    public DateTime DateOfReceipt { get; private set; }
    private readonly List<StockReceiptItem> _items;
    public IReadOnlyCollection<StockReceiptItem> Items => _items.AsReadOnly();

    public StockReceipt(IEnumerable<StockReceiptItem> items)
    {
        if (!items.Any())
        {
            throw new ArgumentException("Receipt item list cannot be empty.");
        }

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
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.");
        }
        if (fulfillableQuantity + defectiveQuantity + warehouseDamagedQuantity == 0)
        {
            throw new ArgumentException("Total receipt quantity cannot be 0.");
        }

        ProductId = productId;
        GoodQuantity = fulfillableQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
    }
}
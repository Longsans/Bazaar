namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockInspectionReport
{
    public DateTime DateConducted { get; private set; }
    public List<StockInspectionItem> Items { get; private set; }

    public StockInspectionReport(IEnumerable<StockInspectionItem> items)
    {
        if (!items.Any())
        {
            throw new ArgumentException("Inspection item list cannot be empty.");
        }
        DateConducted = DateTime.Now.Date;
        Items = items.ToList();

    }
}

public class StockInspectionItem
{
    public string ProductId { get; }
    public uint GoodQuantity { get; }
    public uint DefectiveQuantity { get; }
    public uint WarehouseDamagedQuantity { get; }
    public decimal StorageSpacePerUnitDm3 { get; }
    public decimal AllInspectedItemsStorageSpaceDm3 => StorageSpacePerUnitDm3 * (GoodQuantity + DefectiveQuantity + WarehouseDamagedQuantity);

    public StockInspectionItem(string productId, uint goodQuantity,
        uint defectiveQuantity, uint warehouseDamagedQuantity, decimal storageSpacePerUnitDm3)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.");
        }
        ProductId = productId;
        GoodQuantity = goodQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
        StorageSpacePerUnitDm3 = storageSpacePerUnitDm3;
    }
}
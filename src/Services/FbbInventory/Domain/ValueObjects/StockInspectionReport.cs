namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockInspectionReport
{
    public DateTime DateConducted { get; set; }
    public List<StockInspectionItem> Items { get; set; }

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

    public StockInspectionItem(string productId, uint goodQuantity,
        uint defectiveQuantity, uint warehouseDamagedQuantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.");
        }
        ProductId = productId;
        GoodQuantity = goodQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
    }
}
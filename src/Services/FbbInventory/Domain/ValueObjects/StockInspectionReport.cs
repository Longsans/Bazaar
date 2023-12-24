namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class StockInspectionReport
{
    public DateTime DateConducted { get; set; }
    public List<StockInspectionItem> Items { get; set; }

    public StockInspectionReport(IEnumerable<StockInspectionItem> items)
    {
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
        ProductId = productId;
        GoodQuantity = goodQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
    }
}
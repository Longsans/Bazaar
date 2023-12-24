namespace Bazaar.FbbInventory.Domain.Services;

public class FixedQualityInspectionService : IQualityInspectionService
{
    private readonly uint _fixedDefectiveQtyPerItem;
    private readonly uint _fixedWarehouseDamagedQtyPerItem;

    public FixedQualityInspectionService(IConfiguration config)
    {
        _fixedDefectiveQtyPerItem = uint.Parse(config["QaDefectivePerItem"]!);
        _fixedWarehouseDamagedQtyPerItem = uint.Parse(config["QaWarehouseDamagedPerItem"]!);
    }

    public StockInspectionReport ConductInspection(IEnumerable<InboundStockQuantity> inspectQuantities)
    {
        var reportItems = inspectQuantities.Select(x =>
        {
            var goodQty = x.Quantity;
            uint defectiveQty = 0;
            uint warehouseDmgQty = 0;
            if (goodQty > _fixedDefectiveQtyPerItem)
            {
                defectiveQty = _fixedDefectiveQtyPerItem;
                goodQty -= defectiveQty;
            }
            if (goodQty > _fixedWarehouseDamagedQtyPerItem)
            {
                warehouseDmgQty = _fixedWarehouseDamagedQtyPerItem;
                goodQty -= warehouseDmgQty;
            }
            return new StockInspectionItem(x.ProductId, goodQty, defectiveQty, warehouseDmgQty);
        });
        return new StockInspectionReport(reportItems);
    }
}

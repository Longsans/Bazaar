namespace Bazaar.FbbInventory.Domain.Services;

public class FixedStockInspectionService : IStockInspectionService
{
    private readonly uint _fixedDefectiveQtyPerItem;
    private readonly uint _fixedWarehouseDamagedQtyPerItem;
    private readonly IRepository<ProductInventory> _inventoryRepo;

    public FixedStockInspectionService(IConfiguration config, IRepository<ProductInventory> inventoryRepo)
    {
        _fixedDefectiveQtyPerItem = uint.Parse(config["QaDefectivePerItem"]!);
        _fixedWarehouseDamagedQtyPerItem = uint.Parse(config["QaWarehouseDamagedPerItem"]!);
        _inventoryRepo = inventoryRepo;
    }

    public async Task<StockInspectionReport> ConductInspection(IEnumerable<InboundStockQuantity> inspectQuantities)
    {
        var areBooks = (await _inventoryRepo.ListAsync()).Count <= 4; // First 4 seeded products are books
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

            var prodWidthCm = areBooks ? 16m : 100m;
            var prodLengthCm = areBooks ? 24m : 100m;
            var prodHeightCm = areBooks ? 5m : 100m;
            var storageSpaceInDm3 = prodWidthCm * prodLengthCm * prodHeightCm / 1000m;
            return new StockInspectionItem(x.ProductId, goodQty, defectiveQty, warehouseDmgQty, storageSpaceInDm3);
        });
        return new StockInspectionReport(reportItems);
    }
}

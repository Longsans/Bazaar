namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IQualityInspectionService
{
    StockInspectionReport ConductInspection(
        IEnumerable<InboundStockQuantity> inspectQuantities);
}

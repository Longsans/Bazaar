namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IStockInspectionService
{
    Task<StockInspectionReport> ConductInspection(
        IEnumerable<InboundStockQuantity> inspectQuantities);
}

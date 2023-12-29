namespace Bazaar.Catalog.Web.Messages;

public record ChangeStockRequest(StockChangeType ChangeType, uint Units);

public enum StockChangeType
{
    Reduce,
    Restock,
}
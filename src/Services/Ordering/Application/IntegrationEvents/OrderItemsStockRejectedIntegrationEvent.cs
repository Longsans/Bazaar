namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderItemsStockRejectedIntegrationEvent(
    int OrderId, IEnumerable<StockRejectedOrderItem> StockRejectedItems) : IntegrationEvent;

public record StockRejectedOrderItem(string ProductId,
    uint OrderedQuantity, uint? InStockQuantity, StockRejectionReason RejectionReason);

public enum StockRejectionReason
{
    NoListing,
    InsufficientStock
}
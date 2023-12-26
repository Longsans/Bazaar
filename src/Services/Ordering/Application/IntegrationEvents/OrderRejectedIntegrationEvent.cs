namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderRejectedIntegrationEvent(
    int OrderId,
    string BuyerId,
    IEnumerable<StockRejectedOrderItem> StockRejectedItems
) : IntegrationEvent;

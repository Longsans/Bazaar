namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderPlacedIntegrationEvent(
    int OrderId, IEnumerable<OrderStockItem> OrderItems) : IntegrationEvent;

public readonly record struct OrderStockItem(string ProductId, uint Quantity);
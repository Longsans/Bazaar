namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderCreatedIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;

public readonly record struct OrderStockItem(string ProductId, uint Quantity);
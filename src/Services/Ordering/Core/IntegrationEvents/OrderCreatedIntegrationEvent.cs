namespace Bazaar.Ordering.Core.IntegrationEvents;

public record OrderCreatedIntegrationEvent(int orderId, IEnumerable<OrderStockItem> orderStockItems) : IntegrationEvent;

public readonly record struct OrderStockItem(string productId, int quantity);
namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record OrderCreatedIntegrationEvent(
    int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;

public readonly record struct OrderStockItem(string ProductId, uint Quantity);
namespace Bazaar.Catalog.Domain.IntegrationEvents;

public record OrderCreatedIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;

public readonly record struct OrderStockItem(string ProductId, int Quantity);
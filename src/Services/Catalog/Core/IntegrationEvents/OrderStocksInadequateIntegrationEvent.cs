namespace Bazaar.Catalog.Core.IntegrationEvents;

public record OrderStocksInadequateIntegrationEvent(int OrderId, IEnumerable<OrderStockInadequateItem> OrderStockInadequateItems) : IntegrationEvent;

public readonly record struct OrderStockInadequateItem(string ProductId, int OrderedQuantity, int AvailableStock);

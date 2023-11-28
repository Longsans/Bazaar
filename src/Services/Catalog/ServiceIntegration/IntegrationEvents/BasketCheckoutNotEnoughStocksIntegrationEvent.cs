namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record BasketCheckoutNotEnoughStocksIntegrationEvent(
    string BuyerId, IEnumerable<CheckoutItemWithoutEnoughStock> OrderStockInadequateItems) : IntegrationEvent;

public readonly record struct CheckoutItemWithoutEnoughStock(
    string ProductId, uint PurchaseQuantity, uint AvailableStock);

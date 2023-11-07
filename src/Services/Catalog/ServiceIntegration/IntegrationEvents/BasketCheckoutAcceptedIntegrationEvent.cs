namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record BasketCheckoutAcceptedIntegrationEvent(
    string BuyerId, IEnumerable<CheckoutEventBasketItem> BasketItems) : IntegrationEvent;

public record CheckoutEventBasketItem(
    string ProductId, string ProductName, decimal UnitPrice, uint Quantity);
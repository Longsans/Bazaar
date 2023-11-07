namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record BasketCheckoutStocksConfirmedIntegrationEvent(string BuyerId) : IntegrationEvent;

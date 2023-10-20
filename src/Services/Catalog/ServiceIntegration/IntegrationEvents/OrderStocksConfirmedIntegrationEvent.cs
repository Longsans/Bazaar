namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;

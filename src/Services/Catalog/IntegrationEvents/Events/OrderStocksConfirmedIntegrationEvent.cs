namespace Bazaar.Catalog.IntegrationEvents.Events;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;

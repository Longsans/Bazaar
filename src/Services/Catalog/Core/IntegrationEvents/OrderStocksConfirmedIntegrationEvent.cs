namespace Bazaar.Catalog.Core.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;

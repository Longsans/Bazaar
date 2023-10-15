namespace Bazaar.Catalog.Domain.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;

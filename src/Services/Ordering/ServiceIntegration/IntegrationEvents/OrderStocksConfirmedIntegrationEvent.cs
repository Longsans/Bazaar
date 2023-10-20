namespace Bazaar.Ordering.ServiceIntegration.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
namespace Bazaar.Ordering.Domain.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
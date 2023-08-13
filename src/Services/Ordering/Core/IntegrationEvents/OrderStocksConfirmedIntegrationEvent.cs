namespace Bazaar.Ordering.Core.IntegrationEvents;

public record OrderStocksConfirmedIntegrationEvent(int orderId) : IntegrationEvent;
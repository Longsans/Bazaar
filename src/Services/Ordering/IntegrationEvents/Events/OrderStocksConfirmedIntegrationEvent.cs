namespace Bazaar.Ordering.Events;

public record OrderStocksConfirmedIntegrationEvent(int orderId) : IntegrationEvent;
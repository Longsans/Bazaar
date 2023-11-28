namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductOrdersStatusReportChangedIntegrationEvent(
    string ProductId, uint OrdersInProgress,
    uint ShippedOrders, uint CancelledOrders) : IntegrationEvent;

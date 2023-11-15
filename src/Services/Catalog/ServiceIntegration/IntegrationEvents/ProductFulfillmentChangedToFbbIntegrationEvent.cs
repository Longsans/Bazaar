namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentChangedToFbbIntegrationEvent(
    string ProductId) : IntegrationEvent;

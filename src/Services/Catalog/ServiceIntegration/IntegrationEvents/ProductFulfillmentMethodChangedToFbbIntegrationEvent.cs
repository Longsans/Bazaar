namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentMethodChangedToFbbIntegrationEvent(
    string ProductId) : IntegrationEvent;

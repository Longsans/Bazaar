namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductFulfillmentMethodChangedToFbbIntegrationEvent(
    string ProductId) : IntegrationEvent;

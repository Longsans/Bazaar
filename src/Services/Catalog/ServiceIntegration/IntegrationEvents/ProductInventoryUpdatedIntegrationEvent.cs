namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductInventoryUpdatedIntegrationEvent(
    string ProductId, uint UpdatedStock) : IntegrationEvent;

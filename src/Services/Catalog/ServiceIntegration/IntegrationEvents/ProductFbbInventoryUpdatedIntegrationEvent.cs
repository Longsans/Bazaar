namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryUpdatedIntegrationEvent(
    string ProductId, uint UpdatedStock) : IntegrationEvent;

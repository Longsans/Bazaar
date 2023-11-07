namespace Bazaar.Inventory.ServiceIntegration.IntegrationEvents;

public record ProductInventoryUpdatedIntegrationEvent(
    string ProductId, uint UpdatedStock) : IntegrationEvent;

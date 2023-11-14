namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryUpdatedIntegrationEvent(
    string ProductId, uint UpdatedStock) : IntegrationEvent;

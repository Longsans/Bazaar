namespace Bazaar.Inventory.ServiceIntegration.IntegrationEvents;

public record ProductInventoryDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

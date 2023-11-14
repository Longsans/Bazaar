namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

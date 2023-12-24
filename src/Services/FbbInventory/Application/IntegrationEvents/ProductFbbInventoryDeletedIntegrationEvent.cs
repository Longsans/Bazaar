namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductFbbInventoryDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

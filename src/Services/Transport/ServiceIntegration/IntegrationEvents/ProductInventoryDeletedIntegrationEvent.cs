namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record ProductInventoryDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

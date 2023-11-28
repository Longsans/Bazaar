namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record CatalogItemDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

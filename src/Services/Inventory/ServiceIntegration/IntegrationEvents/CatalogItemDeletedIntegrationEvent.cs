namespace Bazaar.Inventory.ServiceIntegration.IntegrationEvents;

public record CatalogItemDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

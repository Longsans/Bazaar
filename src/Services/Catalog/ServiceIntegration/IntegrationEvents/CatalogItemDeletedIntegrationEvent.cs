namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record CatalogItemDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

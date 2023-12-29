namespace Bazaar.Catalog.Application.IntegrationEvents;

public record CatalogItemDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductListingDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

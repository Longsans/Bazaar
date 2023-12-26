namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductListingClosedIntegrationEvent(string ProductId) : IntegrationEvent;

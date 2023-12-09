namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductListingClosedIntegrationEvent(string ProductId) : IntegrationEvent;

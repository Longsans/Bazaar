namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductRelistedIntegrationEvent(string ProductId) : IntegrationEvent;

namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductRelistedIntegrationEvent(string ProductId) : IntegrationEvent;

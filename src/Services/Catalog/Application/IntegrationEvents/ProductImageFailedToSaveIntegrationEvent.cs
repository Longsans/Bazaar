namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductImageFailedToSaveIntegrationEvent(string ProductId, string FailureMessage) : IntegrationEvent;

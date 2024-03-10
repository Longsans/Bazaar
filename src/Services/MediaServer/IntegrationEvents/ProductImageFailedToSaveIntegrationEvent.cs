namespace Bazaar.MediaServer.IntegrationEvents;

public record ProductImageFailedToSaveIntegrationEvent(string ProductId, string FailureMessage) : IntegrationEvent;

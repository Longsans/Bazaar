namespace Bazaar.MediaServer.IntegrationEvents;

public record ProductImageUpdatedIntegrationEvent(string ProductId, string Base64ImageString) : IntegrationEvent;

namespace Bazaar.MediaServer.IntegrationEvents;

public record ProductImageUpdatedIntegrationEvent(string ProductId, string Base64EncodedImage) : IntegrationEvent;

namespace Bazaar.MediaServer.IntegrationEvents;

public record ProductImageSavedIntegrationEvent(string ProductId, string ImageUrl) : IntegrationEvent;

namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductImageUpdatedIntegrationEvent(string ProductId, string Base64EncodedImage) : IntegrationEvent;

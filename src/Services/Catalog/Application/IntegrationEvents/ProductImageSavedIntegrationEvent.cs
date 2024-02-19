namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductImageSavedIntegrationEvent(string ProductId, string ImageUrl) : IntegrationEvent;

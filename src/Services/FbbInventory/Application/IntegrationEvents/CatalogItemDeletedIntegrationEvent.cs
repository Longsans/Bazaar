namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record CatalogItemDeletedIntegrationEvent(string ProductId) : IntegrationEvent;

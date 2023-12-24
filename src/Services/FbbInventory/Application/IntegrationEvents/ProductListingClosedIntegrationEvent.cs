namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductListingClosedIntegrationEvent(string ProductId) : IntegrationEvent;

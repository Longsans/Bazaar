namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductListingClosedIntegrationEvent(string ProductId) : IntegrationEvent;

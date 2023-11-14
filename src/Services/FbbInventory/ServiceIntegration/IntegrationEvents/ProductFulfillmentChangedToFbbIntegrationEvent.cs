namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentChangedToFbbIntegrationEvent(
    string ProductId) : IntegrationEvent;

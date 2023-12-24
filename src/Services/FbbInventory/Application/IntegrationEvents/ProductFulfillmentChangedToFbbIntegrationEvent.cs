namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductFulfillmentChangedToFbbIntegrationEvent(
    string ProductId) : IntegrationEvent;

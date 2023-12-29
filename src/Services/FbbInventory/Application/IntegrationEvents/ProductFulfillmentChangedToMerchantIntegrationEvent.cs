namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductFulfillmentChangedToMerchantIntegrationEvent(
    string ProductId) : IntegrationEvent;

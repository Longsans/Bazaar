namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentChangedToMerchantIntegrationEvent(
    string ProductId) : IntegrationEvent;

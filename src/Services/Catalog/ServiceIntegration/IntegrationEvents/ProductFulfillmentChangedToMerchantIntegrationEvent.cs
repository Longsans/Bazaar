namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentChangedToMerchantIntegrationEvent(
    string ProductId) : IntegrationEvent;

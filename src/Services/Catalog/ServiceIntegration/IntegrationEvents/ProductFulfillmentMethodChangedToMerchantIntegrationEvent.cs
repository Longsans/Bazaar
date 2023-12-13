namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductFulfillmentMethodChangedToMerchantIntegrationEvent(
    string ProductId) : IntegrationEvent;

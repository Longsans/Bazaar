namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductFulfillmentMethodChangedToMerchantIntegrationEvent(
    string ProductId) : IntegrationEvent;

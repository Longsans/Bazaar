namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record BasketCheckoutItemsUnavailableIntegrationEvent(
    string BuyerId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

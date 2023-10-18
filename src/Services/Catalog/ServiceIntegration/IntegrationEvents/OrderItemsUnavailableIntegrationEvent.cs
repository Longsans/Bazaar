namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

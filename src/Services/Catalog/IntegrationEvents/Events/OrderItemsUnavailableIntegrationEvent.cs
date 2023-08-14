namespace Bazaar.Catalog.IntegrationEvents.Events;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

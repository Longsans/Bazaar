namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

namespace Bazaar.Ordering.Domain.IntegrationEvents;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

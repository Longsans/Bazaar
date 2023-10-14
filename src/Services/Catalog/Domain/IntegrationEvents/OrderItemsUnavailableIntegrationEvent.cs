namespace Bazaar.Catalog.Domain.IntegrationEvents;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

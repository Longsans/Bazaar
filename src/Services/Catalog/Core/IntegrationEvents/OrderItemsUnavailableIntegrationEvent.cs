namespace Bazaar.Catalog.Core.IntegrationEvents;

public record OrderItemsUnavailableIntegrationEvent(int OrderId, IEnumerable<string> UnavailableProductIds) : IntegrationEvent;

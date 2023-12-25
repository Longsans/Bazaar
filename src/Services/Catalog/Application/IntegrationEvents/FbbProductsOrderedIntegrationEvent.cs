namespace Bazaar.Catalog.Application.IntegrationEvents;

public record FbbProductsOrderedIntegrationEvent(
    int OrderId, IEnumerable<OrderedFbbProduct> OrderedProducts) : IntegrationEvent;

public record OrderedFbbProduct(string ProductId, uint OrderedQuantity);
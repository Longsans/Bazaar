namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductsHaveOrdersInProgressIntegrationEvent(
    IEnumerable<string> ProductIds, string SellerId) : IntegrationEvent;

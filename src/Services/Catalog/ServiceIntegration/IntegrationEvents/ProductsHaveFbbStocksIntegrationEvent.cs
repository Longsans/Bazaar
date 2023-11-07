namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductsHaveFbbStocksIntegrationEvent(
    IEnumerable<string> ProductIds, string SellerId) : IntegrationEvent;

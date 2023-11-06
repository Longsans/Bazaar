namespace Bazaar.Contracting.ServiceIntegration.IntegrationEvents;

public record ProductsHaveFbbStocksIntegrationEvent(
    IEnumerable<string> ProductIds, string SellerId) : IntegrationEvent;

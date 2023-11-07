namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ProductsHaveOrdersInProgressIntegrationEvent(
    IEnumerable<string> ProductIds, string SellerId) : IntegrationEvent;

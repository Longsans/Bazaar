namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record ClientAccountClosedIntegrationEvent(string ClientId) : IntegrationEvent;

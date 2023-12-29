namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ClientAccountClosedIntegrationEvent(string ClientId) : IntegrationEvent;

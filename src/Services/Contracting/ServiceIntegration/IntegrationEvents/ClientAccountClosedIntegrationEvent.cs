namespace Bazaar.Contracting.ServiceIntegration.IntegrationEvents;

public record ClientAccountClosedIntegrationEvent(string ClientId) : IntegrationEvent;

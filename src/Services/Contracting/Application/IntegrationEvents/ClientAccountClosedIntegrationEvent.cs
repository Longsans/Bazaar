namespace Bazaar.Contracting.Application.IntegrationEvents;

public record ClientAccountClosedIntegrationEvent(string ClientId) : IntegrationEvent;

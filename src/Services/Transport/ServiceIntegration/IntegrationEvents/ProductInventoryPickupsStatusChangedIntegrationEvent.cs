namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record ProductInventoryPickupsStatusChangedIntegrationEvent(
    string ProductId, uint ScheduledPickups, uint InProgressPickups,
    uint CompletedPickups, uint CancelledPickups) : IntegrationEvent;

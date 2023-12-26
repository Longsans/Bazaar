namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryPickupsStatusChangedIntegrationEvent(
    string ProductId, uint ScheduledPickups, uint InProgressPickups,
    uint CompletedPickups, uint CancelledPickups) : IntegrationEvent;

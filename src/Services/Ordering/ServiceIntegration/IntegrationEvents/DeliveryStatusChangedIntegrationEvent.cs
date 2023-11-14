namespace Bazaar.Ordering.ServiceIntegration.IntegrationEvents;

public record DeliveryStatusChangedIntegrationEvent(
    int DeliveryId, int OrderId, DeliveryStatus Status) : IntegrationEvent;

public enum DeliveryStatus
{
    Scheduled,
    Delivering,
    Completed,
    Postponed,
    Cancelled
}

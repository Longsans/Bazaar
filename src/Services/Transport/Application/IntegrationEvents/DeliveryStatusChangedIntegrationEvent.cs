namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record DeliveryStatusChangedIntegrationEvent(
    int DeliveryId, int OrderId, DeliveryStatus Status) : IntegrationEvent;

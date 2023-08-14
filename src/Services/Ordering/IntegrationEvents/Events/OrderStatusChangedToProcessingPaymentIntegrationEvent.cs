namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int OrderId) : IntegrationEvent;
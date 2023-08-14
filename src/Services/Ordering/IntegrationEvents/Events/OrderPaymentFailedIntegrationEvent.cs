namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;
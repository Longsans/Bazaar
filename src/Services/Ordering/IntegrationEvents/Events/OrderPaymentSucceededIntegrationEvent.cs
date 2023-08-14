namespace Bazaar.Ordering.IntegrationEvents.Events;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;
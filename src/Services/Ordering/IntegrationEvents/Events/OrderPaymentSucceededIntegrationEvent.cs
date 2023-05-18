namespace Bazaar.Ordering.Events;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;
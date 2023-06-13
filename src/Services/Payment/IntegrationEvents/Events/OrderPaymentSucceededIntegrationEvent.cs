namespace Bazaar.Payment.Events;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;
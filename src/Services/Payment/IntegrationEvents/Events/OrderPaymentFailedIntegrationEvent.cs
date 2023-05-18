namespace Bazaar.Payment.Events;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;
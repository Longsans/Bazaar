namespace Bazaar.Payment.Events;

public record OrderPaymentSucceededIntegrationEvent(int OrderId) : IntegrationEvent;
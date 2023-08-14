namespace Bazaar.Payment.Events;

public record OrderPaymentFailedIntegrationEvent(int OrderId) : IntegrationEvent;
namespace Bazaar.Payment.Application.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(int OrderId) : IntegrationEvent;
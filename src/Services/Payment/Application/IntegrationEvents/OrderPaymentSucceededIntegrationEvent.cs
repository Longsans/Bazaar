namespace Bazaar.Payment.Application.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(int OrderId) : IntegrationEvent;
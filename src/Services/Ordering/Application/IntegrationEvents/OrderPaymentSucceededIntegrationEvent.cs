namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;
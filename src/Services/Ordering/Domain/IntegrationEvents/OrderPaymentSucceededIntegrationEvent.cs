namespace Bazaar.Ordering.Domain.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;